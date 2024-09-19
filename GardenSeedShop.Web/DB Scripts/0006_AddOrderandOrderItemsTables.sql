CREATE TABLE orders (
    id INT NOT NULL PRIMARY KEY IDENTITY,
	client_id INT NOT NULL,
    order_date DATETIME NOT NULL,
	shipping_fee DECIMAL (16, 2) NOT NULL,
	delivery_address VARCHAR (255) NOT NULL,
	payment_method VARCHAR (50) NOT NULL,
	payment_status VARCHAR(20) NOT NULL CHECK (payment_status IN('pending', 'accepted', 'canceled')),
	order_status VARCHAR(20) NOT NULL CHECK (
		order_status IN('created', 'accepted', 'canceled', 'shipped', 'delivered', 'returned')
	)
);

CREATE TABLE order_items (
    id INT NOT NULL PRIMARY KEY IDENTITY,
	order_id INT NOT NULL,
	book_id INT NOT NULL,
    quantity INT NOT NULL,
	unit_price DECIMAL (16, 2) NOT NULL,
);