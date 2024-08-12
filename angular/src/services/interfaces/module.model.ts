import * as moment from 'moment';

export class Module {
  id: number;
  moduleGroupId: number;
  name: string;
  code: string;
  description: string;
  sortOrder: number;
  accessTypeId: number;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() { }
}
