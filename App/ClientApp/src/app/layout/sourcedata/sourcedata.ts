

export class SourceData {
  "Identifier": number;
  "StatementId": number;
  "CustomerId": number;
  "CustomerName": string;
  "PageId": number;
  "PageName": string;
  "WidgetId": number;
  "Widgetname": string;
  "EventDate": Date;
  "EventType": string;
  "AccountId": string;
}

export class WidgetVisitorPieChartData {
  "name": string;
  "y": number;
}
export class PageWidgetVistorData {
  "values": string[];
  "widgetNames": number[];
}
export class VisitorForDay {
  "time": any[];
  "values": number[];
}
export class DatewiseVisitorSeries {
  "name":string;
  "data": number[];
}
export class DatewiseVisitor {
  "dates": string[];
  "datewiseVisitorSeries": DatewiseVisitorSeries[];
}

