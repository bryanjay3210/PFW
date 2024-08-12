import * as moment from 'moment';

export class Contact {
  id: number;
  customerId: number;
  locationId: number;
  positionTypeId: number;
  contactName: string;
  phoneNumber: string;
  email: string;
  notes: string;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() { }
}
