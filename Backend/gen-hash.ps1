Add-Type -Path "C:\Users\sksam\.nuget\packages\bcrypt.net-next\4.0.3\lib\net6.0\BCrypt.Net-Next.dll"
$h1 = [BCrypt.Net.BCrypt]::HashPassword("Password123")
$h2 = [BCrypt.Net.BCrypt]::HashPassword("Admin@123")
Write-Host "User hash: $h1"
Write-Host "Admin hash: $h2"
