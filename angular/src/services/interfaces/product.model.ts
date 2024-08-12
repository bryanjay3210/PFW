import * as moment from 'moment';

export class Product {
  id: number;
  partNumber: number;
  partDescription: string;
  brand: string;

  yearModelRangeFrom: number;
  yearModelRangeTo: number;

  priceLevel1: number;
  priceLevel2: number;
  priceLevel3: number;
  priceLevel4: number;
  priceLevel5: number;
  priceLevel6: number;
  priceLevel7: number;
  priceLevel8: number;

  oemId: number;
  oem: string;
  oemListPrice: number;

  previousCost: number;
  currentCost: number;

  partslinkId: number;
  partslinkNumber: string;

  imageUrl: string;
  image: any;

  categoryId: number;
  sequenceId: number;
  statusId: number;
  partSizeId: number;
  partSize: string;

  onReceivingHold: number;
  onOrder: number;

  isDropShipAllowed: number;
  isWebsiteOption: boolean;

  dateAdded: moment.Moment;
  
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;

  constructor() {}
}
