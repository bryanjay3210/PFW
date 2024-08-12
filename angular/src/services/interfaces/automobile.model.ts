import * as moment from 'moment';

export class Automobile {
  id: number;
  name: string;
  make: string;
  model: string;
  type: string;
  year: number;
  notes: string;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() {}
}
