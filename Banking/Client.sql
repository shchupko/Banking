
CREATE TABLE sizes
(
    name varchar(10) NOT NULL CHECK (name IN('small', 'medium', 'large'))
)

use BankingDb
CREATE TABLE Clients
(
	[ContactNumber] INT NOT NULL PRIMARY KEY IDENTITY,
	[Firstname] NVARCHAR(100) NOT NULL,
    [Lastname] NCHAR(100) NOT NULL, 
    [Birsday] DATETIME NOT NULL, 
    [Phone] NCHAR(100) NOT NULL, 
    [Status] NCHAR(100) NOT NULL, 
    [Depo] BIT NOT NULL
)

use BankingDb
CREATE TABLE Users
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Login] NVARCHAR(100) NOT NULL,
    [Password] NCHAR(100) NOT NULL, 
    [Lastname] NCHAR(100) NULL, 
    [Email] NCHAR(100) NULL, 
    [Cookies] NCHAR(100) NULL, 
	[AttemptCounter] INT NULL, 
	[RegDate] DATETIME NULL, 
    [IsBlock] BIT NOT NULL,
	[isConfirmedEmail] BIT NOT NULL,
	[Guid] NCHAR(100) NULL
)

INSERT INTO Users( Login, Password) 
VALUES 
	('Login1', 'Password1'),
	('Login2', 'Password2'),
	('Login3', 'Password3'),
	('Login4', 'Password4'),
	('Login5', 'Password5'),
	('Login6', 'Password6'),
	('Login7', 'Password7');
	
INSERT INTO Clients( Firstname, Lastname, Depo, Birsday, Status) 
VALUES 
	('Firstname1', 'Lastname1', 'True', '1980-07-20', 'VIP'),
	('Firstname2', 'Lastname2', 'True', '1980-07-20', 'VIP'),
	('Firstname3', 'Lastname3', 'True', '1980-07-20', 'VIP'),
	('Firstname4', 'Lastname4', 'False', '1980-07-20', 'Clasic'),
	('Firstname5', 'Lastname5', 'True', '1980-07-20', 'VIP'),
	('Firstname6', 'Lastname6', 'True', '1980-07-20', 'VIP'),
	('Firstname7', 'Lastname7', 'True', '1980-07-20', 'VIP');