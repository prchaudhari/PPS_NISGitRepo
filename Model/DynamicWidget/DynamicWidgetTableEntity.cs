// <copyright file="DynamicWidgetTableEntity.cs" company="Websym Solutions Pvt. Ltd.">
// Copyright (c) 2018 Websym Solutions Pvt. Ltd..
// </copyright>
//-----------------------------------------------------------------------

using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;

namespace nIS
{
    public class DynamicWidgetTableEntity
    {

        public string HeaderName;
        public string FieldId;
        public string FieldName;
        public string AccountNumber;
        public string IsSorting;

    }
    public class DynamicWidgetFormEntity
    {
        public string DisplayName;
        public string FieldId;
        public string FieldName;
    }
    public class ChartSeries
    {
        public string name;
        public IList<decimal> data;
        public string type;
    }
    public class DynamicWidgetLineGraph
    {
        public IList<DynamicWidgetFormEntity> Details = new List<DynamicWidgetFormEntity>();
        public string XAxis = string.Empty;
    }
    public class GraphChartData
    {
        public ChartTitle title;
        public IList<ChartSeries> series=new List<ChartSeries>();
        public IList<string> xAxis=new List<string>();
        public string color;

    }
    public class ChartTitle
    {
        public string text;
    }
    public class CustomeTheme
    {
        public string TitleColor;
        public string TitleSize;
        public string TitleWeight;
        public string TitleType;
        public string HeaderColor;
        public string HeaderSize;
        public string HeaderWeight;
        public string HeaderType;
        public string DataColor;
        public string DataSize;
        public string DataWeight;
        public string DataType;
        public string ChartColorTheme;
        public string ColorTheme;
    }
    public class PieChartSeries
    { 
        public string name;
        public IList<PieChartData> data=new List<PieChartData>();
    }
    public class PieChartData
    {
        public string name;
        public decimal y;
    }
    public class PiChartGraphData
    { 
        public IList<PieChartSeries> series=new List<PieChartSeries>();
        public ChartTitle title;
        public string color;
    }
    public class PieChartSettingDetails
    {
        public string PieSeries;
        public string PieValue;
        public string PieSeriesName;
    }

}
