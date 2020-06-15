
--- India States

Declare @MaharashtraStateIdentifier as bigint
Insert into nIS.State values('Maharashtra',@IndiaCountryIdentifier,1,0)
Set @MaharashtraStateIdentifier = @@IDENTITY

Declare @GujaratStateIdentifier as bigint
Insert into nIS.State values('Gujarat',@IndiaCountryIdentifier,1,0)
Set @GujaratStateIdentifier = @@IDENTITY

Declare @TamilNaduStateIdentifier as bigint
Insert into nIS.State values('Tamil Nadu',@IndiaCountryIdentifier,1,0)
Set @TamilNaduStateIdentifier = @@IDENTITY

Declare @UttarPradeshStateIdentifier as bigint
Insert into nIS.State values('Uttar Pradesh',@IndiaCountryIdentifier,1,0)
Set @UttarPradeshStateIdentifier = @@IDENTITY

--- USA Stetes

Declare @AlabamaStateIdentifier as bigint
Insert into nIS.State values('Alabama',@USACountryIdentifier,1,0)
Set @AlabamaStateIdentifier = @@IDENTITY

Declare @AlaskaStateIdentifier as bigint
Insert into nIS.State values('Alaska',@USACountryIdentifier,1,0)
Set @AlaskaStateIdentifier = @@IDENTITY

Declare @CaliforniaStateIdentifier as bigint
Insert into nIS.State values('California',@USACountryIdentifier,1,0)
Set @CaliforniaStateIdentifier = @@IDENTITY

Declare @FloridaStateIdentifier as bigint
Insert into nIS.State values('Florida',@USACountryIdentifier,1,0)
Set @FloridaStateIdentifier = @@IDENTITY

--- UAE

Declare @AbuDhabiStateIdentifier as bigint
Insert into nIS.State values('Abu dhabi',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @AbuDhabiStateIdentifier = @@IDENTITY

Declare @DubaiStateIdentifier as bigint
Insert into nIS.State values('Dubai',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @DubaiStateIdentifier = @@IDENTITY

Declare @AjmanStateIdentifier as bigint
Insert into nIS.State values('Ajman',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @AjmanStateIdentifier = @@IDENTITY

Declare @FujairahStateIdentifier as bigint
Insert into nIS.State values('Fujairah',@UnitedArabEmiratesCountryIdentifier,1,0)
Set @FujairahStateIdentifier = @@IDENTITY