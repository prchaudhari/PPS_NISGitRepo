import { FilterPipe } from './filter.pipe';

describe('FilterPipe', () => {
  let pipe: FilterPipe;
  it('create an instance', () => {
    const pipe = new FilterPipe();
    expect(pipe).toBeTruthy();
  });

  beforeEach(() => {
    pipe = new FilterPipe();
});

it('providing no value returns fallback', () => {
  const items = [];
    items.push({});

  let searchText: 
  {EntityName : 'Allowance',
   Operation:'Create' }
  //  let mockEntity :{
  //   EntityName: 'Allowance',
  //   Operation:'Create' 
  //  }

   const filtered = pipe.transform(items, searchText);

  expect(filtered.length).toBe(1);
  // expect(pipe.transform(items,searchText)).toBe(mockEntity);
});
});
