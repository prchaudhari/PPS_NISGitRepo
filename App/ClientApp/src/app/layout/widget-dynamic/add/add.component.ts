import { Component, OnInit, ViewChild } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Router } from '@angular/router';
import { AngularEditorConfig } from '@kolkov/angular-editor';
export interface ListElement {
  series: string;
  displayName: string;
}

const List_Data: ListElement[] = [
  { series: 'Field Name 01', displayName: 'FN 01'},
  { series: 'Field Name 02', displayName: 'FN 02'},
  { series: 'Field Name 03', displayName: 'FN 03'},
  { series: 'Field Name 04', displayName: 'FN 04'},
  { series: 'Field Name 05', displayName: 'FN 05'},
];
@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit {
  //html editor code
  htmlContent = '';
  config: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: '15rem',
    minHeight: '5rem',
    placeholder: 'Enter text here...',
    translate: 'no',
    defaultParagraphSeparator: 'p',
    defaultFontName: 'Arial',
    toolbarHiddenButtons: [
      //['bold']
    ],
    customClasses: [
      {
        name: "quote",
        class: "quote",
      },
      {
        name: 'redText',
        class: 'redText'
      },
      {
        name: "titleText",
        class: "titleText",
        tag: "h1",
      },
    ]
  };
  public isDefault: boolean = true;
  public isCustome: boolean = false;
  isDefaultClicked() {
    this.isDefault = true;
    this.isCustome = false;
  }
  isCustomeClicked() {
    this.isDefault = false;
    this.isCustome = true;
  }

  public isTheme1Active: boolean = false;
  public isTheme2Active: boolean = false;
  public isTheme3Active: boolean = false;
  public isTheme4Active: boolean = false;
  public isTheme5Active: boolean = false;
  public isTheme0Active: boolean = true;
  //Functions call to click the theme of the page--
  theme1() {
    this.isTheme1Active = true;
    this.isTheme2Active = false;
    this.isTheme3Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme2() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = true;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme3() {
    this.isTheme1Active = false;
    this.isTheme3Active = true;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = false;

  }
  theme4() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = true;
    this.isTheme5Active = false;
    this.isTheme0Active = false;
  }
  theme5() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = true;
    this.isTheme0Active = false;
  }
  theme0() {
    this.isTheme1Active = false;
    this.isTheme3Active = false;
    this.isTheme2Active = false;
    this.isTheme4Active = false;
    this.isTheme5Active = false;
    this.isTheme0Active = true;
  }
  public pageSize = 5;
  public currentPage = 0;
  public totalSize = 0;

  displayedColumns: string[] = ['series', 'displayName', 'actions'];
  dataSource = new MatTableDataSource<ListElement>(List_Data);

  @ViewChild(MatSort, { static: true }) sort: MatSort;
  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  public handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
  }

  tableHeader = [
    {fn:'Field Name 01', name:'Date',sorting : 'Yes'},
    {fn:'Field Name 02', name: 'Amout', sorting: 'No' },
    {fn:'Field Name 03', name:'Narration', sorting : 'Yes'},
    {fn:'Field Name 04', name:'Balance', sorting : 'No'},
    {fn:'Field Name 05', name: 'Product', sorting: 'Yes'},
    {fn:'Field Name 06', name: 'User', sorting: 'Yes'},
    {fn:'Field Name 07', name: 'Role', sorting: 'Yes'},
  ];

  formList = [
    { fn: 'Field Name 01', dn: 'Customer Name' },
    { fn: 'Field Name 02', dn: 'Customer Id' },
    { fn: 'Field Name 03', dn: 'Balance' },
    { fn: 'Field Name 01', dn: 'Account Id' },
  ];

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.tableHeader, event.previousIndex, event.currentIndex);
    moveItemInArray(this.formList, event.previousIndex, event.currentIndex);
  }

  //select widget type radio

   selectedLink: string = "Form";
  setWidgetType(e: string): void {
    //  this.selectRowsOption = '';
    this.selectedLink = e;
  }
  isSelected(name: string): boolean {
    if (!this.selectedLink) { // if no radio button is selected, always return false so every nothing is shown  
      return false;
    }
    return (this.selectedLink === name); // if current radio button is selected, return true, else return false 
  }

 
  
  navigateToListPage() {
    this.route.navigate(['dynamicwidget']);
  }
  constructor(private route: Router) { }

  ngOnInit() {
  }

}
