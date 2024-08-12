import { Module } from './module.model';

export class ModuleGroup {
  id: number;
  name: string;
  code: string;
  description: string;
  sortOrder: number;
  modules: Module[];
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() { }
}
