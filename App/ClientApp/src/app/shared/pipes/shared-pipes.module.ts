import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ArraySortPipePipe } from './array-sort-pipe.pipe';
import { FilterPipe } from './filter.pipe';

@NgModule({
    imports: [
        CommonModule
    ],
    declarations: [ArraySortPipePipe, FilterPipe],
    exports:[
        ArraySortPipePipe,
        FilterPipe
    ],
    providers:[ FilterPipe ]
})
export class SharedPipesModule { }
