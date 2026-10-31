# Fix for Navbar JSON Parse Error

The error occurs because the navbar component is trying to parse undefined/null values as JSON.

## Fix for navbar.component.ts

Replace the isAdmin() method with proper null checking:

```typescript
isAdmin(): boolean {
  try {
    const userStr = localStorage.getItem('user');
    if (!userStr || userStr === 'undefined' || userStr === 'null') {
      return false;
    }
    const user = JSON.parse(userStr);
    return user && user.role === 'Admin';
  } catch (error) {
    console.error('Error parsing user data:', error);
    return false;
  }
}
```

## Alternative safer approach:

```typescript
isAdmin(): boolean {
  try {
    const userStr = localStorage.getItem('user');
    if (userStr && userStr !== 'undefined' && userStr !== 'null') {
      const user = JSON.parse(userStr);
      return user?.role === 'Admin';
    }
    return false;
  } catch {
    return false;
  }
}
```

## Also check other methods that use localStorage:

Make sure any other methods that access localStorage also have proper null checking:

```typescript
isLoggedIn(): boolean {
  try {
    const token = localStorage.getItem('token');
    return token !== null && token !== 'undefined' && token !== '';
  } catch {
    return false;
  }
}

getUser(): any {
  try {
    const userStr = localStorage.getItem('user');
    if (userStr && userStr !== 'undefined' && userStr !== 'null') {
      return JSON.parse(userStr);
    }
    return null;
  } catch {
    return null;
  }
}
```