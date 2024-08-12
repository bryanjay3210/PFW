import * as moment from 'moment';
import { RolePermission } from './rolePermission.model';

export class Role {
  id: number;
  userTypeId: number;
  notes?: string;
  rolePermissions: RolePermission[];
  name: string;
  code: string;
  description?: string;
  sortOrder: number;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy?: string;
  modifiedDate?: moment.Moment;

  constructor() {}
}
