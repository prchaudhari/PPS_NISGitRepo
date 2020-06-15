// <copyright file = "CustomHeaderParameter.cs" company="Websym Solutions Pvt Ltd">
// Copyright (c) 2018 Websym Solutions Pvt Ltd.
// </copyright>


namespace nIS
{
    #region References

    using Swashbuckle.Swagger;
    using System.Collections.Generic;
    using System.Web.Http.Description;

    #endregion

    public class CustomHeaderParameter: IOperationFilter
    {
        public void Apply(Swashbuckle.Swagger.Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
                operation.parameters = new List<Parameter>();

            operation.parameters.Add(new Parameter
            {
                name = "TenantCode",
                @in = "header",
                type = "string",
                required = true
            });
        }
    }
}