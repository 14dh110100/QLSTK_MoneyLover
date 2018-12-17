use master
go
if exists(select * from sysdatabases where name='DbMoneyLover')
	drop database DbMoneyLover
go
create database DbMoneyLover
go
use DbMoneyLover
go

--Create Tables
create table RoleAdmin (
	Id int identity(1,1) not null primary key,
	Acronym nvarchar(10) not null,
	Name nvarchar(20) not null,
	Status int not null
)
go

create table UserAdmin (
	Id int identity(1,1) not null primary key,
	RoleId int not null,
	Acronym nvarchar(10) not null,
	Name nvarchar (50) not null,
	UserName nvarchar(36) null,
	Password nvarchar(36) null,
	Encrypted nvarchar(72) null,
	Status int not null,
	constraint fk_useradmin_roleadmin foreign key (RoleId) references RoleAdmin(Id)
)
go

create table Customer (
	Id int identity(1,1) not null primary key,
	Acronym nvarchar(10) not null,
	Name nvarchar(50) null,
	IdentityCard nvarchar(15) null,
	Adress nvarchar(100) null,
	UserName nvarchar(36) null,
	Password nvarchar(36) null,
	Encrypted nvarchar(72) null,
	Status int not null
)
go

create table Bank (
	Id int identity(1,1) not null primary key,
	Acronym nvarchar(20) not null,
	Name nvarchar(50) not null,
	Status int not null
)
go

create table Term (
	Id int identity(1,1) not null primary key,
	Acronym nvarchar(10) not null,
	Name nvarchar(30) not null,
	MinDate int not null,
	MinPayIn decimal not null,
	Status int not null
)
go

create table InterestRate (
	Id int identity(1,1) not null primary key,
	BankId int not null,
	TermId int not null,
	Acronym nvarchar(30) not null,
	Rate float not null,
	Status int not null,
	constraint fk_interestrate_bank foreign key (BankId) references Bank(Id),
	constraint fk_interestrate_term foreign key (TermId) references Term(Id)
)
go

create table PassBook (
	Id int identity(1,1) not null primary key,
	BankId int not null,
	TermId int not null,
	CustomerId int not null,
	Acronym nvarchar(40) not null,
	Principal decimal not null,
	Balance decimal not null,
	InterestRate float not null,
	DemandInterestRate float not null,
	OpenDate datetime not null,
	ChangeDate datetime not null,
	InterestPayment int not null,
	TermEnd int null,
	Status int not null,
	constraint fk_passbook_bank foreign key (BankId) references Bank(Id),
	constraint fk_passbook_term foreign key (TermId) references Term(Id),
	constraint fk_passbook_customer foreign key (CustomerId) references Customer(Id)
)
go

create table PassbookDetail (
	Id int identity(1,1) not null primary key,
	PassbookId int not null,
	Action nvarchar(15) not null,
	ActionDate datetime not null,
	Balance decimal not null,
	Amount decimal not null,
	Surplus decimal not null,
	Status int not null,
	constraint fk_passbookdetail_passbook foreign key (PassbookId) references PassBook(Id)
)

create table Deposit (
	Id int identity(1,1) not null primary key,
	PassBookId int not null,
	Acronym nvarchar(10) not null,
	DepositDate datetime not null,
	Amount decimal not null,
	Status int not null,
	constraint fk_deposit_passbook foreign key (PassBookId) references PassBook(Id)
)
go

create table Withdraw (
	Id int identity(1,1) not null primary key,
	PassBookId int not null,
	Acronym nvarchar(10) not null,
	WithdrawDate datetime not null,
	Amount decimal not null,
	InterestRate float not null,
	Status int not null,
	constraint fk_withdraw_passbook foreign key (PassBookId) references PassBook(Id)
)
go

--data (insert on sql)
SET IDENTITY_INSERT dbo.Customer ON
INSERT INTO dbo.Customer (Id, Acronym, Name, IdentityCard, Adress, UserName, Password, Encrypted, Status) VALUES (1, N'KH1', N'Nguyễn Văn A', N'123456789', N'TP.HCM', N'user1@gmail.com', N'a@123456', N'021035fc653a80078840586d7720483c', 1)
SET IDENTITY_INSERT dbo.Customer OFF

SET IDENTITY_INSERT dbo.Bank ON
INSERT INTO dbo.Bank (Id, Acronym, Name, Status) VALUES (1, N'ACB', N'Ngân hàng Á Châu', 1)
INSERT INTO dbo.Bank (Id, Acronym, Name, Status) VALUES (2, N'VCB', N'Vietcombank', 1)
INSERT INTO dbo.Bank (Id, Acronym, Name, Status) VALUES (3, N'BIDV', N'BIDV Bank', 1)
INSERT INTO dbo.Bank (Id, Acronym, Name, Status) VALUES (4, N'CNPM', N'Công nghệ phần mềm', 2)
SET IDENTITY_INSERT dbo.Bank OFF

SET IDENTITY_INSERT dbo.Term ON
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (1, N'KKH', N'Không kỳ hạn', 15, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (2, N'1T', N'1 Tháng', 30, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (3, N'3T', N'3 Tháng', 90, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (4, N'6T', N'6 Tháng', 180, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (5, N'12T', N'12 Tháng', 360, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (6, N'KKH', N'Không kỳ hạn', 15, CAST(1000000 AS Decimal(18, 0)), 2)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (7, N'3T', N'3 Tháng', 90, CAST(1000000 AS Decimal(18, 0)), 2)
INSERT INTO dbo.Term (Id, Acronym, Name, MinDate, MinPayIn, Status) VALUES (8, N'6T', N'6 Tháng', 180, CAST(1000000 AS Decimal(18, 0)), 2)
SET IDENTITY_INSERT dbo.Term OFF

SET IDENTITY_INSERT dbo.InterestRate ON
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (1, 1, 1, N'ACB_KKH', 0.3, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (2, 1, 2, N'ACB_1T', 4.7, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (3, 1, 3, N'ACB_3T', 5.0, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (4, 1, 4, N'ACB_6T', 5.5, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (5, 1, 5, N'ACB_12T', 6.2, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (6, 2, 1, N'VCB_KKH', 0.1, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (7, 2, 2, N'VCB_1T', 4.4, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (8, 2, 3, N'VCB_3T', 4.8, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (9, 2, 4, N'VCB_6T', 5.5, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (10, 2, 5, N'VCB_12T', 6.6, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (11, 3, 1, N'BIDV_KKH', 0.2, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (12, 3, 2, N'BIDV_1T', 4.3, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (13, 3, 3, N'BIDV_3T', 4.8, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (14, 3, 4, N'BIDV_6T', 5.3, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (15, 3, 5, N'BIDV_12T', 6.9, 1)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (16, 4, 6, N'CNPM_KKH', 0.15, 2)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (17, 4, 7, N'CNPM_3T', 0.5, 2)
INSERT INTO dbo.InterestRate (Id, BankId, TermId, Acronym, Rate, Status) VALUES (18, 4, 8, N'CNPM_6T', 0.55, 2)
SET IDENTITY_INSERT dbo.InterestRate OFF

SET IDENTITY_INSERT dbo.PassBook ON
INSERT INTO dbo.PassBook (Id, BankId, TermId, CustomerId, Acronym, Principal, Balance, InterestRate, DemandInterestRate, OpenDate, ChangeDate, InterestPayment, TermEnd, Status) VALUES (1, 1, 3, 1, N'ACB_KKH_001', CAST(10000000 AS Decimal(18, 0)), CAST(10000000 AS Decimal(18, 0)), 5.0, 0.3, N'2018-11-02 00:00:00', N'2018-11-02 00:00:00', 1, 1, 1)
SET IDENTITY_INSERT dbo.PassBook OFF

--data (insert on visual studio)
/*SET IDENTITY_INSERT [dbo].[Customer] ON
INSERT INTO [dbo].[Customer] ([Id], [Acronym], [Name], [IdentityCard], [Adress], [UserName], [Password], [Encrypted], [Status]) VALUES (1, N'KH1', N'Nguyễn Văn A', N'123456789', N'TP.HCM', N'user1@gmail.com', N'a@123456', N'021035fc653a80078840586d7720483c', 1)
SET IDENTITY_INSERT [dbo].[Customer] OFF

SET IDENTITY_INSERT [dbo].[Bank] ON
INSERT INTO [dbo].[Bank] ([Id], [Acronym], [Name], [Status]) VALUES (1, N'ACB', N'Ngân hàng Á Châu', 1)
INSERT INTO [dbo].[Bank] ([Id], [Acronym], [Name], [Status]) VALUES (2, N'VCB', N'Vietcombank', 1)
INSERT INTO [dbo].[Bank] ([Id], [Acronym], [Name], [Status]) VALUES (3, N'BIDV', N'BIDV Bank', 1)
SET IDENTITY_INSERT [dbo].[Bank] OFF

SET IDENTITY_INSERT [dbo].[Term] ON
INSERT INTO [dbo].[Term] ([Id], [Acronym], [Name], [MinDate], [MinPayIn], [Status]) VALUES (1, N'KKH', N'Không kỳ hạn', 15, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO [dbo].[Term] ([Id], [Acronym], [Name], [MinDate], [MinPayIn], [Status]) VALUES (2, N'1T', N'1 Tháng', 30, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO [dbo].[Term] ([Id], [Acronym], [Name], [MinDate], [MinPayIn], [Status]) VALUES (3, N'3T', N'3 Tháng', 90, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO [dbo].[Term] ([Id], [Acronym], [Name], [MinDate], [MinPayIn], [Status]) VALUES (4, N'6T', N'6 Tháng', 180, CAST(1000000 AS Decimal(18, 0)), 1)
INSERT INTO [dbo].[Term] ([Id], [Acronym], [Name], [MinDate], [MinPayIn], [Status]) VALUES (5, N'12T', N'12 Tháng', 360, CAST(1000000 AS Decimal(18, 0)), 1)
SET IDENTITY_INSERT [dbo].[Term] OFF

SET IDENTITY_INSERT [dbo].[InterestRate] ON
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (1, 1, 1, N'ACB_KKH', 0.3, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (2, 1, 2, N'ACB_1T', 4.7, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (3, 1, 3, N'ACB_3T', 5.0, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (4, 1, 4, N'ACB_6T', 5.5, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (5, 1, 5, N'ACB_12T', 6.2, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (6, 2, 1, N'VCB_KKH', 0.1, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (7, 2, 2, N'VCB_1T', 4.4, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (8, 2, 3, N'VCB_3T', 4.8, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (9, 2, 4, N'VCB_6T', 5.5, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (10, 2, 5, N'VCB_12T', 6.6, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (11, 3, 1, N'BIDV_KKH', 0.2, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (12, 3, 2, N'BIDV_1T', 4.3, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (13, 3, 3, N'BIDV_3T', 4.8, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (14, 3, 4, N'BIDV_6T', 5.3, 1)
INSERT INTO [dbo].[InterestRate] ([Id], [BankId], [TermId], [Acronym], [Rate], [Status]) VALUES (15, 3, 5, N'BIDV_12T', 6.9, 1)
SET IDENTITY_INSERT [dbo].[InterestRate] OFF

SET IDENTITY_INSERT [dbo].[PassBook] ON
INSERT INTO [dbo].[PassBook] ([Id], [BankId], [TermId], [CustomerId], [Acronym], [Principal], [Balance], [OpenDate], [ChangeDate], [InterestPayment], [TermEnd], [Status]) VALUES (1, 1, 3, 1, N'ACB_KKH_001', CAST(10000000 AS Decimal(18, 0)), CAST(10000000 AS Decimal(18, 0)), N'2018-11-02 00:00:00', N'2018-11-02 00:00:00', 1, 1, 1)
SET IDENTITY_INSERT [dbo].[PassBook] OFF*/
