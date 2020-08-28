import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ArraySortPipePipe } from './array-sort-pipe.pipe';
import { FilterPipe } from './filter.pipe';
import { SafeHtmlPipe } from './safe-html.pipe';
import { DatePipe } from '@angular/common';
@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [ArraySortPipePipe, FilterPipe, SafeHtmlPipe],
    exports:[
        ArraySortPipePipe,
      FilterPipe, DatePipe
    ],
    providers:[ FilterPipe, SafeHtmlPipe ]
})
export class SharedPipesModule { }
