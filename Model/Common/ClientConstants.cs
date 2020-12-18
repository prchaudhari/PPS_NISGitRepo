// <copyright file="ModelConstants.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2016 Websym Solutions Pvt Ltd.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace nIS
{
    public partial class ModelConstant
    {
        #region Client

        /// <summary>
        /// The default role name
        /// </summary>
        public const string DEFAULTROLENAME = "Client Admin";

        /// <summary>
        /// The client model section
        /// </summary>
        public const string CLIENTMODELSECTION = "ClientModel";

        /// <summary>
        /// The client exception section
        /// </summary>
        public const string CLIENTEXCEPTIONSECTION = "ClientException";

        /// <summary>
        /// The client ui section
        /// </summary>
        public const string CLIENTUISECTION = "ClientUI";

        /// <summary>
        /// The client advertiser type
        /// </summary>
        public const string CLIENTADEVERTISERTYPE = "Advertiser";

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> {
                                                            "Manage", "View","Reference"});

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTREFERENCEOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> { "Reference" });

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTVIEWREFERENCEOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> { "View", "Reference" });

        /// <summary>
        /// Customer Producer type entity
        /// </summary>
        public const string ENTITYMANAGE = "Manage";

        /// <summary>
        /// Customer Producer type entity
        /// </summary>
        public const string ENTITYVIEW = "View";

        /// <summary>
        /// Customer Producer type entity
        /// </summary>
        public const string ENTITYREFERENCE = "Reference";
        /// <summary>
        /// The client advertiser type
        /// </summary>
        public const string CLIENTFREESUBSCRIPTION = "Free";

        /// <summary>
        /// The storage migrator.
        /// </summary>
        public const string STORAGEMIGRATOR = "StorageMigrator";

        /// <summary>
        /// The reference operation
        /// </summary>
        public const string ROLEREFERENCEOPEARTION = "Reference";

        /// <summary>
        /// The client ageny type
        /// </summary>
        public const string CLIENTAGENYTYPE = "Agency";

        /// <summary>
        /// The client ageny management
        /// </summary>
        public const string AGENCYVIEWENTITY = "AgencyViewEntity";



        /// <summary>
        /// The client refrence entity
        /// </summary>
        public const string REFERENCEENTITIES = "ReferencesEntity";

        /// <summary>
        /// The client search parameter model section
        /// </summary>
        public const string CLIENTSEARCHPARAMETERMODELSECTION = "ClientSearchParameterModel";

        #region Client UI labels

        /// <summary>
        /// Client add update status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUS = "lblClientAddUpdateStatus";

        /// <summary>
        /// Client add update active status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUSACTIVE = "lblClientAddUpdateStatusActive";

        /// <summary>
        /// Client add update inactive status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUSINACTIVE = "lblClientAddUpdateStatusInactive";

        /// <summary>
        /// Client add update select all checkbox
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTALLCHECKBOX = "lblClientAddUpdateSelectAllCheckbox";

        /// <summary>
        /// Client details title
        /// </summary>
        public const string LBLCLIENTDETAILSTITLE = "lblClientDetailsTitle";

        /// <summary>
        /// The client grid name placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERNAME = "lblClientGridPlaceHolderName";

        /// <summary>
        /// The client grid primary email address placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERPRIMARYEMAILADDRESS = "lblClientGridPlaceHolderPrimaryEmailAddress";

        /// <summary>
        /// The client grid primary contact number placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERPRIMARYCONTACTNUMBER = "lblClientGridPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// The client grid activation status placeholder
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERACTIVATIONSTATUS = "lblClientGridPlaceHolderActivationStatus";

        /// <summary>
        /// Client label title
        /// </summary>
        public const string LBLCLIENTTITLE = "lblClientTitle";

        /// <summary>
        /// Client label table title
        /// </summary>
        public const string LBLCLIENTTABLETITLE = "lblClientTableTitle";

        /// <summary>
        /// Client grid name
        /// </summary>
        public const string LBLCLIENTGRIDNAME = "lblClientGridName";

        /// <summary>
        /// Client grid primary address
        /// </summary>
        public const string LBLCLIENTGRIDPRIMARYEMAIL = "lblClientGridPrimaryEmail";

        /// <summary>
        /// Client grid primary contact
        /// </summary>
        public const string LBLCLIENTGRIDPRIMARYCONTACT = "lblClientGridPrimaryContact";

        /// <summary>
        /// Client grid status
        /// </summary>
        public const string LBLCLIENTGRIDSTATUS = "lblClientGridStatus";

        /// <summary>
        /// Client grid status
        /// </summary>
        public const string LBLCLIENTGRIDACTION = "lblClientGridAction";

        /// <summary>
        /// Client grid Domain name
        /// </summary>
        public const string LBLCLIENTGRIDDOMAINNAME = "lblClientGridDomainName";

        /// <summary>
        /// Client grid status all
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSALL = "lblClientGridStatusAll";

        /// <summary>
        /// Client grid status active
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSACTIVE = "lblClientGridStatusActive";

        /// <summary>
        /// Client grid status inactive
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSINACTIVE = "lblClientGridStatusInactive";

        /// <summary>
        /// Client grid no data to display
        /// </summary>
        public const string LBLCLIENTGRIDNODATATODISPLAY = "lblClientGridNoDataToDisplay";

        /// <summary>
        /// Client grid Previous
        /// </summary>
        public const string LBLCLIENTGRIDPREVIOUS = "lblClientGridPrevious";

        /// <summary>
        /// Client grid Next
        /// </summary>
        public const string LBLCLIENTGRIDNEXT = "lblClientGridNext";

        /// <summary>
        /// Client add title
        /// </summary>
        public const string LBLCLIENTADDTITLE = "lblClientAddTitle";

        /// <summary>
        /// Client update title
        /// </summary>
        public const string LBLCLIENTUPDATETITLE = "lblClientUpdateTitle";

        /// <summary>
        /// Client addUpdate name
        /// </summary>
        public const string LBLCLIENTADDUPDATENAME = "lblClientAddUpdateName";

        /// <summary>
        /// Client addUpdate description
        /// </summary>
        public const string LBLCLIENTADDUPDATEDESCRIPTION = "lblClientAddUpdateDescription";

        /// <summary>
        /// Client addUpdate state
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATE = "lblClientAddUpdateState";

        /// <summary>
        /// Client addUpdate access token
        /// </summary>
        public const string LBLCLIENTADDUPDATEACCESSTOKEN = "lblClientAddUpdateAccessToken";

        /// <summary>
        /// Client addupdate start date
        /// </summary>
        public const string LBLCLIENTADDUPDATESTARTDATE = "lblClientAddUpdateStartDate";

        /// <summary>
        /// Client addupdate end date
        /// </summary>
        public const string LBLCLIENTADDUPDATEENDDATE = "lblClientAddUpdateEndDate";

        /// <summary>
        /// Client addupdate storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATESTORAGEACCOUNT = "lblClientAddUpdateStorageAccount";

        /// <summary>
        /// Client addupdate country
        /// </summary>
        public const string LBLCLIENTADDUPDATECOUNTRY = "lblClientAddUpdateCountry";

        /// <summary>
        /// Client addupdate zip
        /// </summary>
        public const string LBLCLIENTADDUPDATEZIP = "lblClientAddUpdateZip";

        /// <summary>
        /// Client addupdate city
        /// </summary>
        public const string LBLCLIENTADDUPDATECITY = "lblClientAddUpdateCity";

        /// <summary>
        /// Client addupdate select city
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTCITY = "lblClientAddUpdateSelectCity";

        /// <summary>
        /// Client addupdate select state
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTSTATE = "lblClientAddUpdateSelectState";

        /// <summary>
        /// Client addupdate select country
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTCOUNTRY = "lblClientAddUpdateSelectCountry";

        /// <summary>
        /// Client addupdate address
        /// </summary>
        public const string LBLCLIENTADDUPDATEADDRESS = "lblClientAddUpdateAddress";

        /// <summary>
        /// Client addupdate domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDOMAINNAME = "lblClientAddUpdateDomainName";

        /// <summary>
        /// Client addupdate user secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYCONTACTNAME = "lblClientAddUpdateUserSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYCONTACTNAME = "lblClientAddUpdateUserPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user entities
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERENTITIES = "lblClientAddUpdateUserEntities";

        /// <summary>
        /// Client addupdate contact details title
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTDETAILSTITLE = "lblClientAddUpdateContactDetailsTitle";

        /// <summary>
        /// Client addupdate contact primary first name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYFIRSTNAME = "lblClientAddUpdateContactPrimaryFirstName";

        /// <summary>
        /// Client addupdate contact primary last name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYLASTNAME = "lblClientAddUpdateContactPrimaryLastName";

        /// <summary>
        /// Client addupdate contact secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYCONTACTNAME = "lblClientAddUpdateContactSecondaryContactName";

        /// <summary>
        /// Client addupdate contact primary email
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYEMAIL = "lblClientAddUpdateContactPrimaryEmail";

        /// <summary>
        /// Client addupdate contact secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYEMAIL = "lblClientAddUpdateContactSecondaryEmail";

        /// <summary>
        /// Client addupdate contact primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYCONTACTNUMBER = "lblClientAddUpdateContactPrimaryContactNumber";

        /// <summary>
        /// Client addupdate contact secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYCONTACTNUMBER = "lblClientAddUpdateContactSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user details title
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERDETAILSTITLE = "lblClientAddUpdateUserDetailsTitle";

        /// <summary>
        /// Client addupdate contact same as above checkbox
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSAMEASABOVECHECKBOX = "lblClientAddUpdateUserSameAsAbove";

        /// <summary>
        /// Client addupdate user first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERFIRSTNAME = "lblClientAddUpdateUserFirstName";

        /// <summary>
        /// Client addupdate user last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERLASTNAME = "lblClientAddUpdateUserLastName";

        /// <summary>
        /// Client addupdate user secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYEMAIL = "lblClientAddUpdateUserSecondaryEmail";

        /// <summary>
        /// Client addupdate user primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user placeholder contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERCONTACTNUMBER = "lblClientAddUpdateUserPlaceholderContactNumber";

        /// <summary>
        /// Client addupdate user secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user primary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYEMAIL = "lblClientAddUpdateUserPrimaryEmail";

        /// <summary>
        /// Client addupdate placeholder name
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERNAME = "lblClientAddUpdatePlaceHolderName";

        /// <summary>
        /// Client addupdate placeholder description
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERDESCRIPTION = "lblClientAddUpdatePlaceHolderDescription";

        /// <summary>
        /// Client addupdate placeholder access token
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLACCESSTOKEN = "lblClientAddUpdatePlaceHolderAccessToken";

        /// <summary>
        /// Client addupdate placeholder storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERSTORAGEACCOUNT = "lblClientAddUpdatePlaceHolderStorageAccount";

        /// <summary>
        /// Client addupdate placeholder zip code
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERZIPCODE = "lblClientAddUpdatePlaceHolderZipCode";

        /// <summary>
        /// Client addupdate placeholder address
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERADDRESS = "lblClientAddUpdatePlaceHolderAddress";

        /// <summary>
        /// Client addupdate placeholder domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERDOMAINNAME = "lblClientAddUpdatePlaceHolderDomainName";

        /// <summary>
        /// Client addupdate user placeholder first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYFIRSTNAME = "lblClientAddUpdateUserPlaceHolderPrimaryFirstName";

        /// <summary>
        /// Client addupdate user placeholder last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYLASTNAME = "lblClientAddUpdateUserPlaceHolderPrimaryLastName";

        /// <summary>
        /// Client addupdate user placeholder first email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYFIRSTEMAIL = "lblClientAddUpdateUserPlaceHolderPrimaryFirstEmail";

        /// <summary>
        /// Client addupdate user placeholder secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERSECONDARYEMAIL = "lblClientAddUpdateUserPlaceHolderSecondaryEmail";

        /// <summary>
        /// Client addupdate user placeholder primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user placeholder secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERSECONDARYCONTATCNUMBER = "lblClientAddUpdateUserPlaceHolderSecondaryContactNumber";

        /// <summary>
        /// Client addupdate contact placeholder primary first name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYFIRSTNAME = "lblClientAddUpdateContactPlaceHolderPrimaryFirstName";

        /// <summary>
        /// Client addupdate contact placeholder primary last name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYLASTNAME = "lblClientAddUpdateContactPlaceHolderPrimaryLastName";

        /// <summary>
        /// Client addupdate contact placeholder secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYCONTATCNAME = "lblClientAddUpdateContactPlaceHolderSecondaryContactName";

        /// <summary>
        /// Client addupdate contact placeholder primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYFIRSTEMAIL = "lblClientAddUpdateContactPlaceHolderPrimaryFirstEmail";

        /// <summary>
        /// Client addupdate contact placeholder secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYEMAIL = "lblClientAddUpdateContactPlaceHolderSecondaryEmail";

        /// <summary>
        /// Client addupdate contact placeholder primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYCONTATCNUMBER = "lblClientAddUpdateContactPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// Client addupdate contact placeholder secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYCONTATCNUMBER = "lblClientAddUpdateContactPlaceHolderSecondaryContactNumber";

        /// <summary>
        /// Client addupdate details title
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTITLE = "lblClientDetailsTitle";

        /// <summary>
        /// Client addupdate details zip
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSZIP = "lblClientDetailsZip";

        /// <summary>
        /// Client addupdate details domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSDOMAINNAME = "lblClientDetailsDomainName";

        /// <summary>
        /// Client addupdate details name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSNAME = "lblClientDetailsName";

        /// <summary>
        /// Client addupdate details description
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSDESCRIPTION = "lblClientDetailsDescription";

        /// <summary>
        /// Client addupdate details primary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYCONTACTNAME = "lblClientDetailsPrimaryContactName";

        /// <summary>
        /// Client addupdate primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYEMAILADDRESS = "lblClientDetailsPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYCONTACTNUMBERVIEW = "lblClientDetailsPrimaryContactNumber";

        /// <summary>
        /// Client addupdate secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSECONDARYCONTACTNAME = "lblClientDetailsSecondaryContactName";

        /// <summary>
        /// Client addupdate secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSECONDARYMAILADDRESS = "lblClientDetailsSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPSECONDARYCONTACTNUMBER = "lblClientDetailsSecondaryContactNumber";

        /// <summary>
        /// Client addupdate storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSTORAGEACCOUNT = "lblClientDetailsStorageAccount";

        /// <summary>
        /// Client addupdate primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSACCESSTOKEN = "lblClientDetailsAccessToken";

        /// <summary>
        /// Client addupdate details start date
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPSTARTDATE = "lblClientDetailsStartDate";

        /// <summary>
        /// Client addupdate details end date
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSENDDATE = "lblClientDetailsEndDate";

        /// <summary>
        /// Client addupdate details address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSADDRESS = "lblClientDetailsAddress";

        /// <summary>
        /// Client addupdate details city
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCITY = "lblClientDetailsCity";

        /// <summary>
        /// Client addupdate details state
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSTATE = "lblClientDetailsState";

        /// <summary>
        /// Client addupdate details country
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCOUNTRY = "lblClientDetailsCountry";

        /// <summary>
        /// Client addupdate details status
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUS = "lblClientDetailsStatus";

        /// <summary>
        /// Client addupdate details status active
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUSACTIVE = "lblClientDetailsStatusActive";

        /// <summary>
        /// Client addupdate details status inactive
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUSINACTIVE = "lblClientDetailsStatusInactive";

        /// <summary>
        /// Client addupdate details entities
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSENTITIES = "lblClientDetailsEntities";

        /// <summary>
        /// Client addupdate details close
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCLOSE = "lblClientDetailsClose";

        /// <summary>
        /// Client addupdate error name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORNAME = "lblClientAddUpdateErrorName";

        /// <summary>
        /// Client addupdate error max length name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHNAME = "lblClientAddUpdateErrorMaxLengthName";

        /// <summary>
        /// Client addupdate error min length name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHNAME = "lblClientAddUpdateErrorMinLengthName";

        /// <summary>
        /// Client addupdate error domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORDOMAINNAME = "lblClientAddUpdateErrorDomainName";

        /// <summary>
        /// Client addupdate error max length domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHDOMAINNAME = "lblClientAddUpdateErrorMaxLengthDomainName";

        /// <summary>
        /// Client addupdate error min length domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHDOMAINNAME = "lblClientAddUpdateErrorMinLengthDomainName";

        /// <summary>
        /// Client addupdate error max length description message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHDESCRIPTION = "lblClientAddUpdateErrorMaxLengthDescription";

        /// <summary>
        /// Client addupdate error min length description message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHDESCRIPTION = "lblClientAddUpdateErrorMinLengthDescription";

        /// <summary>
        /// Client addupdate error address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORADDRESS = "lblClientAddUpdateErrorAddress";

        /// <summary>
        /// Client addupdate error max length address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHADDRESS = "lblClientAddUpdateErrorMaxLengthAddress";

        /// <summary>
        /// Client addupdate error min length address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHADDRESS = "lblClientAddUpdateErrorMinLengthAddress";

        /// <summary>
        /// Client addupdate error zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORZIPCODE = "lblClientAddUpdateErrorZipcode";

        /// <summary>
        /// Client addupdate error max length zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHZIPCODE = "lblClientAddUpdateErrorMaxLengthZipcode";

        /// <summary>
        /// Client addupdate error min length zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHZIPCODE = "lblClientAddUpdateErrorMinLengthZipcode";

        /// <summary>
        /// Client addupdate error access token message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORACCESSTOKEN = "lblClientAddUpdateErrorAccessToken";

        /// <summary>
        /// Client addupdate error storage account message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORSTORAGEACCOUNT = "lblClientAddUpdateErrorStorageAccount";

        /// <summary>
        /// Client addupdate error start date message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORSTARTDATE = "lblClientAddUpdateErrorStartDate";

        /// <summary>
        /// Client addupdate error end date message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORENDDATE = "lblClientAddUpdateErrorEndDate";

        /// <summary>
        /// Client addupdate contact error first name message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORFIRSTNAME = "lblClientAddUpdateContactErrorFirstName";

        /// <summary>
        /// Client addupdate contact error first name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHFIRSTNAME = "lblClientAddUpdateContactErrorMaxLengthFirstName";

        /// <summary>
        /// Client addupdate contact error first name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHFIRSTNAME = "lblClientAddUpdateContactErrorMinLengthFirstName";

        /// <summary>
        /// Client addupdate contact error last name message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORLASTNAME = "lblClientAddUpdateContactErrorLastName";

        /// <summary>
        /// Client addupdate contact error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHLASTNAME = "lblClientAddUpdateContactErrorMaxLengthLastName";

        /// <summary>
        /// Client addupdate contact error last name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHLASTNAME = "lblClientAddUpdateContactErrorMinLengthLastName";

        /// <summary>
        /// Client addupdate contact error primary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error invalid primary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORINVALIDPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorInvalidPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorMaxLengthPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error domain name does not match exception
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORDOMAINNAMENOTMATCH = "lblClientAddUpdateContactErrorDomainNameNotMatch";

        /// <summary>
        /// Client addupdate contact error mobile number message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMOBILENUMBER = "lblClientAddUpdateContactErrorMobileNumber";

        /// <summary>
        /// Client addupdate contact error mobile number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHMOBILENUMBER = "lblClientAddUpdateContactErrorMaxLengthMobileNumber";

        /// <summary>
        /// Client addupdate contact error mobile number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHMOBILENUMBER = "lblClientAddUpdateContactErrorMinLengthMobileNumber";

        /// <summary>
        /// Client addupdate contact error secondary contact name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYCONTACTNAME = "lblClientAddUpdateContactErrorMaxLengthSecondaryContactName";

        /// <summary>
        /// Client addupdate contact error secondary contact name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHSECONDARYCONTACTNAME = "lblClientAddUpdateContactErrorMinLengthSecondaryContactName";

        /// <summary>
        /// Client addupdate contact error invalid secondary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORINVALIDSECONDARYEMAIL = "lblClientAddUpdateContactErrorInvalidSecondaryEmail";

        /// <summary>
        /// Client addupdate contact error secondary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYEMAIL = "lblClientAddUpdateContactErrorMaxLengthSecondaryEmail";

        /// <summary>
        /// Client addupdate contact error secondary mobile number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYMOBILENUMBER = "lblClientAddUpdateContactErrorMaxLengthSecondaryMobileNumber";

        /// <summary>
        /// Client addupdate contact error secondary mobile number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHSECONDARYMOBILENUMBER = "lblClientAddUpdateContactErrorMinLengthSecondaryMobileNumber";

        /// <summary>
        /// Client addupdate user error first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORFIRSTNAME = "lblClientAddUpdateUserErrorFirstName";

        /// <summary>
        /// Client addupdate user error first name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHFIRSTNAME = "lblClientAddUpdateUserErrorMaxLengthFirstName";

        /// <summary>
        /// Client addupdate user error first name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHFIRSTNAME = "lblClientAddUpdateUserErrorMinLengthFirstName";

        /// <summary>
        /// Client addupdate user error last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORLASTNAME = "lblClientAddUpdateUserErrorLastName";

        /// <summary>
        /// Client addupdate user error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHLASTNAME = "lblClientAddUpdateUserErrorMaxLengthLastName";

        /// <summary>
        /// Client addupdate user error last name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHLASTNAME = "lblClientAddUpdateUserErrorMinLengthLastName";

        /// <summary>
        /// Client addupdate user error primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error invalid primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORINVALIDPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorInvalidPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error primary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorMaxLengthPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error primary email address domain does not match message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYEMAILADDRESSDOMAINNOTMATCH = "lblClientAddUpdateUserErrorPrimaryEmailAddressDomainNotMatch";

        /// <summary>
        /// Client addupdate user error invalid secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORINVALIDSECONDARYEMAILADDRESS = "lblClientAddUpdateUserErrorInvalidSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate user error secondary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHSECONDARYEMAILADDRESS = "lblClientAddUpdateUserErrorMaxLengthSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate user error invalid primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error primary contact number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMaxLengthPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error primary contact number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMinLengthPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error secondary contact number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMaxLengthSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user error secondary contact number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMinLengthSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user error select entity message
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTENTITY = "lblClientAddUpdateUserErrorSelectEntity";

        /// <summary>
        /// Client addupdate user error cancel
        /// </summary>
        public const string LBLCLIENTADDUPDATECANCEL = "lblClientAddUpdateUserCancel";

        /// <summary>
        /// Client addupdate user error save
        /// </summary>
        public const string LBLCLIENTADDUPDATESAVE = "lblClientAddUpdateUserSave";

        #endregion

        #region Client Model section

        /// <summary>
        /// The invalid tenant code
        /// </summary>
        public const string INVALIDTENANTCODE = "InvalidTenantCode";

        /// <summary>
        /// The invalid tenant name
        /// </summary>
        public const string INVALIDTENANTNAME = "InvalidTenantName";

        /// <summary>
        /// The invalid domain name
        /// </summary>
        public const string INVALIDDOMAINNAME = "InvalidDomainName";

        /// <summary>
        /// The invalid primary first name
        /// </summary>
        public const string INVALIDPRIMARYFIRSTNAME = "InvalidPrimaryFirstName";

        /// <summary>
        /// The invalid primary last name
        /// </summary>
        public const string INVALIDPRIMARYLASTNAME = "InvalidPrimaryLastName";

        /// <summary>
        /// The invalid primary contact number
        /// </summary>
        public const string INVALIDPRIMARYCONTACTNUMBER = "InvalidPrimaryContactNumber";

        /// <summary>
        /// The invalid primary email address
        /// </summary>
        public const string INVALIDPRIMARYEMAILADDRESS = "InvalidPrimaryEmailAddress";

        /// <summary>
        /// The invalid primary address line1
        /// </summary>
        public const string INVALIDPRIMARYADDRESSLINE1 = "InvalidPrimaryAddressLineOne";

        /// <summary>
        /// The invalid primary address line1
        /// </summary>
        public const string INVALIDTENANTSUBSCRIPTION = "InvalidTenantSubscription";

        /// <summary>
        /// The invalid primary pin code
        /// </summary>
        public const string INVALIDPRIMARYPINCODE = "InvalidPrimaryPinCode";

        /// <summary>
        /// The invalid start date
        /// </summary>
        public const string INVALIDSTARTDATE = "InvalidStartDate";

        /// <summary>
        /// The invalid end date
        /// </summary>
        public const string INVALIDENDDATE = "InvalidEndDate";

        /// <summary>
        /// The invalid storage account
        /// </summary>
        public const string INVALIDSTORAGEACCOUNT = "InvalidStorageAccount";

        /// <summary>
        /// The invalid access token
        /// </summary>
        public const string INVALIDACCESSTOKEN = "InvalidAccessToken";

        /// <summary>
        /// The invalid client user
        /// </summary>
        public const string INVALIDCLIENTUSER = "InvalidClientUser";

        /// <summary>
        /// The invalid client description
        /// </summary>
        public const string INVALIDCLIENTDESCRIPTION = "InvalidClientDescription";

        /// <summary>
        /// The invalid entities
        /// </summary>
        public const string INVALIDENTITIES = "InvalidEntities";

        /// <summary>
        /// The invalid client paging parameter
        /// </summary>
        public const string INVALIDCLIENTPAGINGPARAMETER = "InvalidClientPagingParameter";

        /// <summary>
        /// The invalid client sort parameter
        /// </summary>
        public const string INVALIDCLIENTSORTPARAMETER = "InvalidClientSortParameter";

        /// <summary>
        /// The invalid manage type.
        /// </summary>
        public const string INVALIDMANAGETYPE = "InvalidManageType";

        /// <summary>
        /// The self managed.
        /// </summary>
        public const string SELFMANAGED = "Self";

        /// <summary>
        /// The managed with optimization.
        /// </summary>
        public const string MANAGEDWITHOPTIMIZATION = "ManageWithOptimization";

        /// <summary>
        /// The managed with optimization.
        /// </summary>
        public const string MANAGEDWITHOUTOPTIMIZATION = "ManageWithoutOptimization";

        #region Clientsubscription history

        /// <summary>
        /// The client subscriptionmodel section
        /// </summary>
        public const string SUBSCRIPTIONHISTORYMODELSECTION = "ClientSubscriptionModel";

        /// <summary>
        /// The invalid client sort parameter
        /// </summary>
        public const string INVALIDSUBSCRIPTIONHISTORYLASTMODIFIEDBY = "InvalidClientSubscriptionlastmodifiedby";

        /// <summary>
        /// The invalid client subscription history
        /// </summary>
        public const string INVALIDTENANTSUBSCRIPTIONHISTORY = "InvalidClientSubscriptionHistory";

        /// <summary>
        /// The invalid client type
        /// </summary>
        public const string INVALIDTENANTTYPE = "InvalidClientType";

        /// <summary>
        /// The invalid client subscriptionidentifier
        /// </summary>
        public const string INVALIDSUBSCRIPTIONHISTORYIDENTIFIER = "InvalidClientSubscriptionIdentifier";


        #endregion

        #endregion

        #region Client UI Message Section

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGDELETECLIENTCONFIRMMESSAGE = "msgDeleteClientConfirmMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTDELETESUCCESSMESSAGE = "msgClientDeleteSuccessMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTADDSUCCESSMESSAGE = "msgClientAddSuccessMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTUPDATESUCCESSMESSAGE = "msgClientUpdateSuccessMessage";

        #endregion

        #endregion
    }
    public partial class ModelConstants
    {
        #region Client

        /// <summary>
        /// The default role name
        /// </summary>
        public const string DEFAULTROLENAME = "Client Admin";

        /// <summary>
        /// The client model section
        /// </summary>
        public const string CLIENTMODELSECTION = "ClientModel";

        /// <summary>
        /// The client exception section
        /// </summary>
        public const string CLIENTEXCEPTIONSECTION = "ClientException";

        /// <summary>
        /// The client ui section
        /// </summary>
        public const string CLIENTUISECTION = "ClientUI";

        /// <summary>
        /// The client advertiser type
        /// </summary>
        public const string CLIENTADEVERTISERTYPE = "Advertiser";

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> {
                                                            "Manage", "View","Reference"});

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTREFERENCEOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> {"Reference"});

        /// <summary>
        /// The client operation based on tenant type
        /// </summary>
        public static readonly IList<String> CLIENTVIEWREFERENCEOPERATION = new ReadOnlyCollection<string>
                                                      (new List<String> {"View", "Reference" });
        /// <summary>
        /// The client advertiser type
        /// </summary>
        public const string CLIENTFREESUBSCRIPTION = "Free";

        /// <summary>
        /// The storage migrator.
        /// </summary>
        public const string STORAGEMIGRATOR = "StorageMigrator";

        /// <summary>
        /// The reference operation
        /// </summary>
        public const string ROLEREFERENCEOPEARTION = "Reference";

        /// <summary>
        /// The client ageny type
        /// </summary>
        public const string CLIENTAGENYTYPE= "Agency";

        /// <summary>
        /// The client ageny management
        /// </summary>
        public const string AGENCYVIEWENTITY = "AgencyViewEntity";

        

        /// <summary>
        /// The client refrence entity
        /// </summary>
        public const string REFERENCEENTITIES = "ReferencesEntity";

        /// <summary>
        /// The client search parameter model section
        /// </summary>
        public const string CLIENTSEARCHPARAMETERMODELSECTION = "ClientSearchParameterModel";

        #region Client UI labels

        /// <summary>
        /// Client add update status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUS = "lblClientAddUpdateStatus";

        /// <summary>
        /// Client add update active status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUSACTIVE = "lblClientAddUpdateStatusActive";

        /// <summary>
        /// Client add update inactive status
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATUSINACTIVE = "lblClientAddUpdateStatusInactive";

        /// <summary>
        /// Client add update select all checkbox
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTALLCHECKBOX = "lblClientAddUpdateSelectAllCheckbox";

        /// <summary>
        /// Client details title
        /// </summary>
        public const string LBLCLIENTDETAILSTITLE = "lblClientDetailsTitle";

        /// <summary>
        /// The client grid name placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERNAME = "lblClientGridPlaceHolderName";

        /// <summary>
        /// The client grid primary email address placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERPRIMARYEMAILADDRESS = "lblClientGridPlaceHolderPrimaryEmailAddress";

        /// <summary>
        /// The client grid primary contact number placeholder 
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERPRIMARYCONTACTNUMBER = "lblClientGridPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// The client grid activation status placeholder
        /// </summary>
        public const string LBLCLIENTGRIDPLACEHOLDERACTIVATIONSTATUS = "lblClientGridPlaceHolderActivationStatus";

        /// <summary>
        /// Client label title
        /// </summary>
        public const string LBLCLIENTTITLE = "lblClientTitle";

        /// <summary>
        /// Client label table title
        /// </summary>
        public const string LBLCLIENTTABLETITLE = "lblClientTableTitle";

        /// <summary>
        /// Client grid name
        /// </summary>
        public const string LBLCLIENTGRIDNAME = "lblClientGridName";

        /// <summary>
        /// Client grid primary address
        /// </summary>
        public const string LBLCLIENTGRIDPRIMARYEMAIL = "lblClientGridPrimaryEmail";

        /// <summary>
        /// Client grid primary contact
        /// </summary>
        public const string LBLCLIENTGRIDPRIMARYCONTACT = "lblClientGridPrimaryContact";

        /// <summary>
        /// Client grid status
        /// </summary>
        public const string LBLCLIENTGRIDSTATUS = "lblClientGridStatus";

        /// <summary>
        /// Client grid status
        /// </summary>
        public const string LBLCLIENTGRIDACTION = "lblClientGridAction";

        /// <summary>
        /// Client grid Domain name
        /// </summary>
        public const string LBLCLIENTGRIDDOMAINNAME = "lblClientGridDomainName";

        /// <summary>
        /// Client grid status all
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSALL = "lblClientGridStatusAll";

        /// <summary>
        /// Client grid status active
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSACTIVE = "lblClientGridStatusActive";

        /// <summary>
        /// Client grid status inactive
        /// </summary>
        public const string LBLCLIENTGRIDSTATUSINACTIVE = "lblClientGridStatusInactive";

        /// <summary>
        /// Client grid no data to display
        /// </summary>
        public const string LBLCLIENTGRIDNODATATODISPLAY = "lblClientGridNoDataToDisplay";

        /// <summary>
        /// Client grid Previous
        /// </summary>
        public const string LBLCLIENTGRIDPREVIOUS = "lblClientGridPrevious";

        /// <summary>
        /// Client grid Next
        /// </summary>
        public const string LBLCLIENTGRIDNEXT = "lblClientGridNext";

        /// <summary>
        /// Client add title
        /// </summary>
        public const string LBLCLIENTADDTITLE = "lblClientAddTitle";

        /// <summary>
        /// Client update title
        /// </summary>
        public const string LBLCLIENTUPDATETITLE = "lblClientUpdateTitle";

        /// <summary>
        /// Client addUpdate name
        /// </summary>
        public const string LBLCLIENTADDUPDATENAME = "lblClientAddUpdateName";

        /// <summary>
        /// Client addUpdate description
        /// </summary>
        public const string LBLCLIENTADDUPDATEDESCRIPTION = "lblClientAddUpdateDescription";

        /// <summary>
        /// Client addUpdate state
        /// </summary>
        public const string LBLCLIENTADDUPDATESTATE = "lblClientAddUpdateState";

        /// <summary>
        /// Client addUpdate access token
        /// </summary>
        public const string LBLCLIENTADDUPDATEACCESSTOKEN = "lblClientAddUpdateAccessToken";

        /// <summary>
        /// Client addupdate start date
        /// </summary>
        public const string LBLCLIENTADDUPDATESTARTDATE = "lblClientAddUpdateStartDate";

        /// <summary>
        /// Client addupdate end date
        /// </summary>
        public const string LBLCLIENTADDUPDATEENDDATE = "lblClientAddUpdateEndDate";

        /// <summary>
        /// Client addupdate storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATESTORAGEACCOUNT = "lblClientAddUpdateStorageAccount";

        /// <summary>
        /// Client addupdate country
        /// </summary>
        public const string LBLCLIENTADDUPDATECOUNTRY = "lblClientAddUpdateCountry";

        /// <summary>
        /// Client addupdate zip
        /// </summary>
        public const string LBLCLIENTADDUPDATEZIP = "lblClientAddUpdateZip";

        /// <summary>
        /// Client addupdate city
        /// </summary>
        public const string LBLCLIENTADDUPDATECITY = "lblClientAddUpdateCity";

        /// <summary>
        /// Client addupdate select city
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTCITY = "lblClientAddUpdateSelectCity";

        /// <summary>
        /// Client addupdate select state
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTSTATE = "lblClientAddUpdateSelectState";

        /// <summary>
        /// Client addupdate select country
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTCOUNTRY = "lblClientAddUpdateSelectCountry";

        /// <summary>
        /// Client addupdate address
        /// </summary>
        public const string LBLCLIENTADDUPDATEADDRESS = "lblClientAddUpdateAddress";

        /// <summary>
        /// Client addupdate domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDOMAINNAME = "lblClientAddUpdateDomainName";

        /// <summary>
        /// Client addupdate user secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYCONTACTNAME = "lblClientAddUpdateUserSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYCONTACTNAME = "lblClientAddUpdateUserPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user entities
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERENTITIES = "lblClientAddUpdateUserEntities";

        /// <summary>
        /// Client addupdate contact details title
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTDETAILSTITLE = "lblClientAddUpdateContactDetailsTitle";

        /// <summary>
        /// Client addupdate contact primary first name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYFIRSTNAME = "lblClientAddUpdateContactPrimaryFirstName";

        /// <summary>
        /// Client addupdate contact primary last name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYLASTNAME = "lblClientAddUpdateContactPrimaryLastName";

        /// <summary>
        /// Client addupdate contact secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYCONTACTNAME = "lblClientAddUpdateContactSecondaryContactName";

        /// <summary>
        /// Client addupdate contact primary email
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYEMAIL = "lblClientAddUpdateContactPrimaryEmail";

        /// <summary>
        /// Client addupdate contact secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYEMAIL = "lblClientAddUpdateContactSecondaryEmail";

        /// <summary>
        /// Client addupdate contact primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPRIMARYCONTACTNUMBER = "lblClientAddUpdateContactPrimaryContactNumber";

        /// <summary>
        /// Client addupdate contact secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTSECONDARYCONTACTNUMBER = "lblClientAddUpdateContactSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user details title
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERDETAILSTITLE = "lblClientAddUpdateUserDetailsTitle";

        /// <summary>
        /// Client addupdate contact same as above checkbox
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSAMEASABOVECHECKBOX = "lblClientAddUpdateUserSameAsAbove";

        /// <summary>
        /// Client addupdate user first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERFIRSTNAME = "lblClientAddUpdateUserFirstName";

        /// <summary>
        /// Client addupdate user last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERLASTNAME = "lblClientAddUpdateUserLastName";

        /// <summary>
        /// Client addupdate user secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYEMAIL = "lblClientAddUpdateUserSecondaryEmail";

        /// <summary>
        /// Client addupdate user primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user placeholder contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERCONTACTNUMBER = "lblClientAddUpdateUserPlaceholderContactNumber";

        /// <summary>
        /// Client addupdate user secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user primary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPRIMARYEMAIL = "lblClientAddUpdateUserPrimaryEmail";

        /// <summary>
        /// Client addupdate placeholder name
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERNAME = "lblClientAddUpdatePlaceHolderName";

        /// <summary>
        /// Client addupdate placeholder description
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERDESCRIPTION = "lblClientAddUpdatePlaceHolderDescription";

        /// <summary>
        /// Client addupdate placeholder access token
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLACCESSTOKEN = "lblClientAddUpdatePlaceHolderAccessToken";

        /// <summary>
        /// Client addupdate placeholder storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERSTORAGEACCOUNT = "lblClientAddUpdatePlaceHolderStorageAccount";

        /// <summary>
        /// Client addupdate placeholder zip code
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERZIPCODE = "lblClientAddUpdatePlaceHolderZipCode";

        /// <summary>
        /// Client addupdate placeholder address
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERADDRESS = "lblClientAddUpdatePlaceHolderAddress";

        /// <summary>
        /// Client addupdate placeholder domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEPLACEHOLDERDOMAINNAME = "lblClientAddUpdatePlaceHolderDomainName";

        /// <summary>
        /// Client addupdate user placeholder first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYFIRSTNAME = "lblClientAddUpdateUserPlaceHolderPrimaryFirstName";

        /// <summary>
        /// Client addupdate user placeholder last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYLASTNAME = "lblClientAddUpdateUserPlaceHolderPrimaryLastName";

        /// <summary>
        /// Client addupdate user placeholder first email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYFIRSTEMAIL = "lblClientAddUpdateUserPlaceHolderPrimaryFirstEmail";

        /// <summary>
        /// Client addupdate user placeholder secondary email
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERSECONDARYEMAIL = "lblClientAddUpdateUserPlaceHolderSecondaryEmail";

        /// <summary>
        /// Client addupdate user placeholder primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user placeholder secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERPLACEHOLDERSECONDARYCONTATCNUMBER = "lblClientAddUpdateUserPlaceHolderSecondaryContactNumber";

        /// <summary>
        /// Client addupdate contact placeholder primary first name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYFIRSTNAME = "lblClientAddUpdateContactPlaceHolderPrimaryFirstName";

        /// <summary>
        /// Client addupdate contact placeholder primary last name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYLASTNAME = "lblClientAddUpdateContactPlaceHolderPrimaryLastName";

        /// <summary>
        /// Client addupdate contact placeholder secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYCONTATCNAME = "lblClientAddUpdateContactPlaceHolderSecondaryContactName";

        /// <summary>
        /// Client addupdate contact placeholder primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYFIRSTEMAIL = "lblClientAddUpdateContactPlaceHolderPrimaryFirstEmail";

        /// <summary>
        /// Client addupdate contact placeholder secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYEMAIL = "lblClientAddUpdateContactPlaceHolderSecondaryEmail";

        /// <summary>
        /// Client addupdate contact placeholder primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERPRIMARYCONTATCNUMBER = "lblClientAddUpdateContactPlaceHolderPrimaryContactNumber";

        /// <summary>
        /// Client addupdate contact placeholder secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTPLACEHOLDERSECONDARYCONTATCNUMBER = "lblClientAddUpdateContactPlaceHolderSecondaryContactNumber";

        /// <summary>
        /// Client addupdate details title
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTITLE = "lblClientDetailsTitle";

        /// <summary>
        /// Client addupdate details zip
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSZIP = "lblClientDetailsZip";

        /// <summary>
        /// Client addupdate details domain name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSDOMAINNAME = "lblClientDetailsDomainName";

        /// <summary>
        /// Client addupdate details name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSNAME = "lblClientDetailsName";

        /// <summary>
        /// Client addupdate details description
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSDESCRIPTION = "lblClientDetailsDescription";

        /// <summary>
        /// Client addupdate details primary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYCONTACTNAME = "lblClientDetailsPrimaryContactName";

        /// <summary>
        /// Client addupdate primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYEMAILADDRESS = "lblClientDetailsPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPRIMARYCONTACTNUMBERVIEW = "lblClientDetailsPrimaryContactNumber";

        /// <summary>
        /// Client addupdate secondary contact name
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSECONDARYCONTACTNAME = "lblClientDetailsSecondaryContactName";

        /// <summary>
        /// Client addupdate secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSECONDARYMAILADDRESS = "lblClientDetailsSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate secondary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPSECONDARYCONTACTNUMBER = "lblClientDetailsSecondaryContactNumber";

        /// <summary>
        /// Client addupdate storage account
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSTORAGEACCOUNT = "lblClientDetailsStorageAccount";

        /// <summary>
        /// Client addupdate primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSACCESSTOKEN = "lblClientDetailsAccessToken";

        /// <summary>
        /// Client addupdate details start date
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSPSTARTDATE = "lblClientDetailsStartDate";

        /// <summary>
        /// Client addupdate details end date
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSENDDATE = "lblClientDetailsEndDate";

        /// <summary>
        /// Client addupdate details address
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSADDRESS = "lblClientDetailsAddress";

        /// <summary>
        /// Client addupdate details city
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCITY = "lblClientDetailsCity";

        /// <summary>
        /// Client addupdate details state
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSSTATE = "lblClientDetailsState";

        /// <summary>
        /// Client addupdate details country
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCOUNTRY = "lblClientDetailsCountry";

        /// <summary>
        /// Client addupdate details status
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUS = "lblClientDetailsStatus";

        /// <summary>
        /// Client addupdate details status active
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUSACTIVE = "lblClientDetailsStatusActive";

        /// <summary>
        /// Client addupdate details status inactive
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSTATUSINACTIVE = "lblClientDetailsStatusInactive";

        /// <summary>
        /// Client addupdate details entities
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSENTITIES = "lblClientDetailsEntities";

        /// <summary>
        /// Client addupdate details close
        /// </summary>
        public const string LBLCLIENTADDUPDATEDETAILSCLOSE = "lblClientDetailsClose";

        /// <summary>
        /// Client addupdate error name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORNAME = "lblClientAddUpdateErrorName";

        /// <summary>
        /// Client addupdate error max length name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHNAME = "lblClientAddUpdateErrorMaxLengthName";

        /// <summary>
        /// Client addupdate error min length name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHNAME = "lblClientAddUpdateErrorMinLengthName";

        /// <summary>
        /// Client addupdate error domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORDOMAINNAME = "lblClientAddUpdateErrorDomainName";

        /// <summary>
        /// Client addupdate error max length domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHDOMAINNAME = "lblClientAddUpdateErrorMaxLengthDomainName";

        /// <summary>
        /// Client addupdate error min length domain name message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHDOMAINNAME = "lblClientAddUpdateErrorMinLengthDomainName";

        /// <summary>
        /// Client addupdate error max length description message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHDESCRIPTION = "lblClientAddUpdateErrorMaxLengthDescription";

        /// <summary>
        /// Client addupdate error min length description message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHDESCRIPTION = "lblClientAddUpdateErrorMinLengthDescription";

        /// <summary>
        /// Client addupdate error address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORADDRESS = "lblClientAddUpdateErrorAddress";

        /// <summary>
        /// Client addupdate error max length address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHADDRESS = "lblClientAddUpdateErrorMaxLengthAddress";

        /// <summary>
        /// Client addupdate error min length address message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHADDRESS = "lblClientAddUpdateErrorMinLengthAddress";

        /// <summary>
        /// Client addupdate error zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORZIPCODE = "lblClientAddUpdateErrorZipcode";

        /// <summary>
        /// Client addupdate error max length zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMAXLENGTHZIPCODE = "lblClientAddUpdateErrorMaxLengthZipcode";

        /// <summary>
        /// Client addupdate error min length zip code message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORMINLENGTHZIPCODE = "lblClientAddUpdateErrorMinLengthZipcode";

        /// <summary>
        /// Client addupdate error access token message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORACCESSTOKEN = "lblClientAddUpdateErrorAccessToken";

        /// <summary>
        /// Client addupdate error storage account message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORSTORAGEACCOUNT = "lblClientAddUpdateErrorStorageAccount";

        /// <summary>
        /// Client addupdate error start date message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORSTARTDATE = "lblClientAddUpdateErrorStartDate";

        /// <summary>
        /// Client addupdate error end date message
        /// </summary>
        public const string LBLCLIENTADDUPDATEERRORENDDATE = "lblClientAddUpdateErrorEndDate";

        /// <summary>
        /// Client addupdate contact error first name message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORFIRSTNAME = "lblClientAddUpdateContactErrorFirstName";

        /// <summary>
        /// Client addupdate contact error first name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHFIRSTNAME = "lblClientAddUpdateContactErrorMaxLengthFirstName";

        /// <summary>
        /// Client addupdate contact error first name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHFIRSTNAME = "lblClientAddUpdateContactErrorMinLengthFirstName";

        /// <summary>
        /// Client addupdate contact error last name message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORLASTNAME = "lblClientAddUpdateContactErrorLastName";

        /// <summary>
        /// Client addupdate contact error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHLASTNAME = "lblClientAddUpdateContactErrorMaxLengthLastName";

        /// <summary>
        /// Client addupdate contact error last name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHLASTNAME = "lblClientAddUpdateContactErrorMinLengthLastName";

        /// <summary>
        /// Client addupdate contact error primary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error invalid primary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORINVALIDPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorInvalidPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHPRIMARYEMAILADDRESS = "lblClientAddUpdateContactErrorMaxLengthPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate contact error domain name does not match exception
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORDOMAINNAMENOTMATCH = "lblClientAddUpdateContactErrorDomainNameNotMatch";

        /// <summary>
        /// Client addupdate contact error mobile number message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMOBILENUMBER = "lblClientAddUpdateContactErrorMobileNumber";

        /// <summary>
        /// Client addupdate contact error mobile number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHMOBILENUMBER = "lblClientAddUpdateContactErrorMaxLengthMobileNumber";

        /// <summary>
        /// Client addupdate contact error mobile number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHMOBILENUMBER = "lblClientAddUpdateContactErrorMinLengthMobileNumber";

        /// <summary>
        /// Client addupdate contact error secondary contact name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYCONTACTNAME = "lblClientAddUpdateContactErrorMaxLengthSecondaryContactName";

        /// <summary>
        /// Client addupdate contact error secondary contact name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHSECONDARYCONTACTNAME = "lblClientAddUpdateContactErrorMinLengthSecondaryContactName";

        /// <summary>
        /// Client addupdate contact error invalid secondary email address message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORINVALIDSECONDARYEMAIL = "lblClientAddUpdateContactErrorInvalidSecondaryEmail";

        /// <summary>
        /// Client addupdate contact error secondary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYEMAIL = "lblClientAddUpdateContactErrorMaxLengthSecondaryEmail";

        /// <summary>
        /// Client addupdate contact error secondary mobile number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMAXLENGTHSECONDARYMOBILENUMBER = "lblClientAddUpdateContactErrorMaxLengthSecondaryMobileNumber";

        /// <summary>
        /// Client addupdate contact error secondary mobile number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATECONTACTERRORMINLENGTHSECONDARYMOBILENUMBER = "lblClientAddUpdateContactErrorMinLengthSecondaryMobileNumber";

        /// <summary>
        /// Client addupdate user error first name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORFIRSTNAME = "lblClientAddUpdateUserErrorFirstName";

        /// <summary>
        /// Client addupdate user error first name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHFIRSTNAME = "lblClientAddUpdateUserErrorMaxLengthFirstName";

        /// <summary>
        /// Client addupdate user error first name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHFIRSTNAME = "lblClientAddUpdateUserErrorMinLengthFirstName";

        /// <summary>
        /// Client addupdate user error last name
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORLASTNAME = "lblClientAddUpdateUserErrorLastName";

        /// <summary>
        /// Client addupdate user error last name max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHLASTNAME = "lblClientAddUpdateUserErrorMaxLengthLastName";

        /// <summary>
        /// Client addupdate user error last name min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHLASTNAME = "lblClientAddUpdateUserErrorMinLengthLastName";

        /// <summary>
        /// Client addupdate user error primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error invalid primary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORINVALIDPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorInvalidPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error primary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHPRIMARYEMAILADDRESS = "lblClientAddUpdateUserErrorMaxLengthPrimaryEmailAddress";

        /// <summary>
        /// Client addupdate user error primary email address domain does not match message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYEMAILADDRESSDOMAINNOTMATCH = "lblClientAddUpdateUserErrorPrimaryEmailAddressDomainNotMatch";

        /// <summary>
        /// Client addupdate user error invalid secondary email address
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORINVALIDSECONDARYEMAILADDRESS = "lblClientAddUpdateUserErrorInvalidSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate user error secondary email address max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHSECONDARYEMAILADDRESS = "lblClientAddUpdateUserErrorMaxLengthSecondaryEmailAddress";

        /// <summary>
        /// Client addupdate user error invalid primary contact number
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error primary contact number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMaxLengthPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error primary contact number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHPRIMARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMinLengthPrimaryContactNumber";

        /// <summary>
        /// Client addupdate user error secondary contact number max length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMAXLENGTHSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMaxLengthSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user error secondary contact number min length message
        /// </summary>
        public const string LBLCLIENTADDUPDATEUSERERRORMINLENGTHSECONDARYCONTACTNUMBER = "lblClientAddUpdateUserErrorMinLengthSecondaryContactNumber";

        /// <summary>
        /// Client addupdate user error select entity message
        /// </summary>
        public const string LBLCLIENTADDUPDATESELECTENTITY = "lblClientAddUpdateUserErrorSelectEntity";

        /// <summary>
        /// Client addupdate user error cancel
        /// </summary>
        public const string LBLCLIENTADDUPDATECANCEL = "lblClientAddUpdateUserCancel";

        /// <summary>
        /// Client addupdate user error save
        /// </summary>
        public const string LBLCLIENTADDUPDATESAVE = "lblClientAddUpdateUserSave";

        #endregion

        #region Client Model section

        /// <summary>
        /// The invalid tenant code
        /// </summary>
        public const string INVALIDTENANTCODE = "InvalidTenantCode";

        /// <summary>
        /// The invalid tenant name
        /// </summary>
        public const string INVALIDTENANTNAME = "InvalidTenantName";

        /// <summary>
        /// The invalid domain name
        /// </summary>
        public const string INVALIDDOMAINNAME = "InvalidDomainName";

        /// <summary>
        /// The invalid primary first name
        /// </summary>
        public const string INVALIDPRIMARYFIRSTNAME = "InvalidPrimaryFirstName";

        /// <summary>
        /// The invalid primary last name
        /// </summary>
        public const string INVALIDPRIMARYLASTNAME = "InvalidPrimaryLastName";

        /// <summary>
        /// The invalid primary contact number
        /// </summary>
        public const string INVALIDPRIMARYCONTACTNUMBER = "InvalidPrimaryContactNumber";

        /// <summary>
        /// The invalid primary email address
        /// </summary>
        public const string INVALIDPRIMARYEMAILADDRESS = "InvalidPrimaryEmailAddress";

        /// <summary>
        /// The invalid primary address line1
        /// </summary>
        public const string INVALIDPRIMARYADDRESSLINE1 = "InvalidPrimaryAddressLineOne";

        /// <summary>
        /// The invalid primary pin code
        /// </summary>
        public const string INVALIDPRIMARYPINCODE = "InvalidPrimaryPinCode";

        /// <summary>
        /// The invalid start date
        /// </summary>
        public const string INVALIDSTARTDATE = "InvalidStartDate";

        /// <summary>
        /// The invalid end date
        /// </summary>
        public const string INVALIDENDDATE = "InvalidEndDate";

        /// <summary>
        /// The invalid storage account
        /// </summary>
        public const string INVALIDSTORAGEACCOUNT = "InvalidStorageAccount";

        /// <summary>
        /// The invalid access token
        /// </summary>
        public const string INVALIDACCESSTOKEN = "InvalidAccessToken";

        /// <summary>
        /// The invalid client user
        /// </summary>
        public const string INVALIDCLIENTUSER = "InvalidClientUser";

        /// <summary>
        /// The invalid client description
        /// </summary>
        public const string INVALIDCLIENTDESCRIPTION = "InvalidClientDescription";

        /// <summary>
        /// The invalid entities
        /// </summary>
        public const string INVALIDENTITIES = "InvalidEntities";

        /// <summary>
        /// The invalid client paging parameter
        /// </summary>
        public const string INVALIDCLIENTPAGINGPARAMETER = "InvalidClientPagingParameter";

        /// <summary>
        /// The invalid client sort parameter
        /// </summary>
        public const string INVALIDCLIENTSORTPARAMETER = "InvalidClientSortParameter";

        /// <summary>
        /// The invalid manage type.
        /// </summary>
        public const string INVALIDMANAGETYPE = "InvalidManageType";

        /// <summary>
        /// The self managed.
        /// </summary>
        public const string SELFMANAGED = "Self";

        /// <summary>
        /// The managed with optimization.
        /// </summary>
        public const string MANAGEDWITHOPTIMIZATION = "ManageWithOptimization";

        /// <summary>
        /// The managed with optimization.
        /// </summary>
        public const string MANAGEDWITHOUTOPTIMIZATION = "ManageWithoutOptimization";

        #region Clientsubscription history

        /// <summary>
        /// The client subscriptionmodel section
        /// </summary>
        public const string SUBSCRIPTIONHISTORYMODELSECTION = "ClientSubscriptionModel";

        /// <summary>
        /// The invalid client sort parameter
        /// </summary>
        public const string INVALIDSUBSCRIPTIONHISTORYLASTMODIFIEDBY = "InvalidClientSubscriptionlastmodifiedby";

        /// <summary>
        /// The invalid client subscription history
        /// </summary>
        public const string INVALIDTENANTSUBSCRIPTIONHISTORY = "InvalidClientSubscriptionHistory";

        /// <summary>
        /// The invalid client type
        /// </summary>
        public const string INVALIDTENANTTYPE = "InvalidClientType";

        /// <summary>
        /// The invalid client subscriptionidentifier
        /// </summary>
        public const string INVALIDSUBSCRIPTIONHISTORYIDENTIFIER = "InvalidClientSubscriptionIdentifier";
        

        #endregion

        #endregion

        #region Client UI Message Section

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGDELETECLIENTCONFIRMMESSAGE = "msgDeleteClientConfirmMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTDELETESUCCESSMESSAGE = "msgClientDeleteSuccessMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTADDSUCCESSMESSAGE = "msgClientAddSuccessMessage";

        /// <summary>
        /// Client confirm message
        /// </summary>
        public const string MSGCLIENTUPDATESUCCESSMESSAGE = "msgClientUpdateSuccessMessage";

        #endregion

        #endregion
    }
}
