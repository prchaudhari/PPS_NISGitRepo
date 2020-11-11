// <copyright file="DynamicWidgetFilterEntity.cs" company="Websym Solutions Pvt. Ltd.">
////Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

namespace nIS
{
    #region References
    using System;
    using System.ComponentModel;
    #endregion

    /// <summary>
    /// This class represents dynamic widget filter entity model.
    /// </summary>
    /// 
    public class DynamicWidgetFilterEntity
    {
        public string FieldId { get; set; }
        public string Operator { get; set; }
        public string ConditionalOperator { get; set; }
        public int Sequence { get; set; }
        public string Value { get; set; }
        public string OperatorName { get; set; }
        public string FieldName { get; set; }
    }
}
