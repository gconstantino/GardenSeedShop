ALTER TABLE seeds
ADD price decimal (16,2) NULL



UPDATE seeds SET price = ROUND(2 + (RAND(CHECKSUM(NEWID())) * (4 - 2)), 3);
