import { Pipe, PipeTransform } from '@angular/core';



@Pipe({
    name: 'filterAuthorisedRole'
})
export class FilterPipe implements PipeTransform {

    //Items array and search test is coming from the respective html page where filter pipe is passed--
    transform(items: any[], searchText: { EntityName: string, Operation: string }): any {
        if (items) {
            var entity = items.filter((item: any) => item.EntityName === searchText.EntityName && item.Operation === searchText.Operation)
            if (entity.length > 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}