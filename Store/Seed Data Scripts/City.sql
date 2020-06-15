
--- India 
Go 
Declare @MumbaiCityIdentifier as bigint
Insert into nIS.City values('Mumbai',@MaharashtraStateIdentifier,1,0)
Set @MumbaiCityIdentifier = @@IDENTITY

Declare @PuneCityIdentifier as bigint
Insert into nIS.City values('Pune',@MaharashtraStateIdentifier,1,0)
Set @PuneCityIdentifier = @@IDENTITY

Declare @AhmedabadCityIdentifier as bigint
Insert into nIS.City values('Ahmedabad',@GujaratStateIdentifier,1,0)
Set @AhmedabadCityIdentifier = @@IDENTITY

Declare @ChennaiCityIdentifier as bigint
Insert into nIS.City values('Chennai',@TamilNaduStateIdentifier,1,0)
Set @ChennaiCityIdentifier = @@IDENTITY

Declare @GhaziabadCityIdentifier as bigint
Insert into nIS.City values('Ghaziabad',@UttarPradeshStateIdentifier,1,0)
Set @GhaziabadCityIdentifier = @@IDENTITY

--- USA

Declare @PhenixCityIdentifier as bigint
Insert into nIS.City values('Phenix', @AlabamaStateIdentifier,1,0)
Set @PhenixCityIdentifier = @@IDENTITY

Declare @AnchorageCityIdentifier as bigint
Insert into nIS.City values('Anchorage', @AlaskaStateIdentifier,1,0)
Set @AnchorageCityIdentifier = @@IDENTITY

Declare @AdelantoCityIdentifier as bigint
Insert into nIS.City values('Adelanto', @CaliforniaStateIdentifier,1,0)
Set @AdelantoCityIdentifier = @@IDENTITY

Declare @PascoCityIdentifier as bigint
Insert into nIS.City values('Pasco', @FloridaStateIdentifier,1,0)
Set @PascoCityIdentifier = @@IDENTITY

--- UAE

Declare @DubaiCityIdentifier as bigint
Insert into nIS.City values('Dubai', @DubaiStateIdentifier,1,0)
Set @DubaiCityIdentifier = @@IDENTITY