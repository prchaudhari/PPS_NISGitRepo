import { NgModule, NO_ERRORS_SCHEMA, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { WidgetDynamicRoutingModule } from './widget-dynamic-routing.module';
import { ListComponent } from './list/list.component';
import { AddComponent } from './add/add.component';
import { ViewComponent } from './view/view.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { OwlDateTimeModule, OwlNativeDateTimeModule } from 'ng-pick-datetime';
import { RichTextEditorModule } from '@syncfusion/ej2-angular-richtexteditor';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { WidgetdesignerComponent } from './widgetdesigner/widgetdesigner.component';
import { SharedModule } from '../../shared/modules/shared.module';
@NgModule({
  declarations: [ListComponent, AddComponent, ViewComponent, WidgetdesignerComponent],
  imports: [
    CommonModule,
    WidgetDynamicRoutingModule, MatSortModule, MatTableModule, MatPaginatorModule,
    DragDropModule, OwlDateTimeModule, OwlNativeDateTimeModule, AngularEditorModule,
    FormsModule, ReactiveFormsModule, HttpClientModule, RichTextEditorModule, SharedModule
  ],
  schemas: [
    CUSTOM_ELEMENTS_SCHEMA,
    NO_ERRORS_SCHEMA
  ]
})
export class WidgetDynamicModule { }
