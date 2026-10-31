-- Update existing bookings to have proper TravelClass values
UPDATE Bookings SET TravelClass = 1 WHERE TravelClass = 0;

-- Update existing passengers to have proper TravelClass values  
UPDATE Passengers SET TravelClass = 1 WHERE TravelClass = 0;

-- Update existing seats to have proper SeatClass values
UPDATE Seats SET SeatClass = 1 WHERE SeatClass = 0;