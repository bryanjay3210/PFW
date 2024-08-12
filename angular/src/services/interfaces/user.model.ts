import * as moment from 'moment';
import { Role } from './role.model';

export class User_Old {
  id: number;
  //userTypeId: number;
  roleId: number;
  role: Role;
  customerId?: number;
  locationId?: number;
  userName: string;
  email: string;
  isCustomerUser: boolean;
  isActivated: boolean;
  isChangePasswordOnLogin: boolean;
  passwordHash: string;
  passwordSalt: string;
  
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() {}
}
