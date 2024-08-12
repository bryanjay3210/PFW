import moment from "moment";

export class Customer {
  id: number;
  accountNumber: number;
  customerName: string;
  customerTypeId: number;
  priceLevelId: number;
  paymentTermId: number;
  creditLimit: number;
  taxRate: number;
  overBalance: number
  isHoldAccount: boolean;
  discount: number;
  sellersPermit: string;
  crossStreet: string;
  salesRepresentativeOutId: number;
  salesRepresentativeInId: number;

  contactName: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
  phoneNumber: string;
  faxNumber: string;
  email: string;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;
}
