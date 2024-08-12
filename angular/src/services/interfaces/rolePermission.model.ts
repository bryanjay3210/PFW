import * as moment from 'moment';
import { Module } from './module.model';
import { ModuleGroup } from './moduleGroup.model';

export class RolePermission {
  id: number;
  roleId: number;
  moduleGroupId: number;
  moduleGroup: ModuleGroup;
  moduleId: number;
  module: Module;
  accessTypeId: number;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() {}
}
