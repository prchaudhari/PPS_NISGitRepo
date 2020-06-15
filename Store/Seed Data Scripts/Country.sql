Declare @IndiaCountryIdentifier as bigint
Insert into nIS.Country values('India','IN','+91',1,0)
Set @IndiaCountryIdentifier = @@IDENTITY

Declare @USACountryIdentifier as bigint
Insert into nIS.Country values('USA','US','+1',1,0)
Set @USACountryIdentifier = @@IDENTITY

Declare @UnitedArabEmiratesCountryIdentifier as bigint
Insert into nIS.Country values('UAE','UAE','+971',1,0)
Set @UnitedArabEmiratesCountryIdentifier = @@IDENTITY