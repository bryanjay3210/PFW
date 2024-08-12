import moment from "moment";

export class Location_Old {
  id: number;
  customerId: number;
  locationTypeId: number;
  locationName: string;
  locationCode: string;
  addressLine1: string;
  addressLine2: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
  phoneNumber: string;
  faxNumber: string;
  email: string;
  latitude: string;
  longitude: string;
  notes: string;
  isActive: boolean;
  isDeleted: boolean;
  createdBy: string;
  createdDate: moment.Moment;
  modifiedBy: string;
  modifiedDate: moment.Moment;
}
