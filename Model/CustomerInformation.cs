// <copyright file="CustomerInformation.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    #endregion

    public class CustomerInformation
    {
        #region Public Member
        public string TITLE_TEXT { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string FIRST_NAME_TEXT { get; set; }
        public string SURNAME_TEXT { get; set; }
        public string ADDR_LINE_0 { get; set; }
        public string ADDR_LINE_1 { get; set; }
        public string ADDR_LINE_2 { get; set; }
        public string ADDR_LINE_3 { get; set; }
        public string ADDR_LINE_4 { get; set; }
        #endregion
    }
}
