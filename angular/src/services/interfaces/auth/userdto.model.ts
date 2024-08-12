import * as moment from 'moment';

export class UserDTO_Old {
  userName: string;
  email: string;
  roleId: number;
  password: string;
  isCustomerUser: boolean;
  customerId?: number;
  locationId?: number;
  createdBy: string;
  createdDate: moment.Moment;

  constructor() {}
}
