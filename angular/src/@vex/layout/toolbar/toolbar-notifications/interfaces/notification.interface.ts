import { DateTime } from 'luxon';

export interface Notification {
  id: string;
  icon: string;
  label: string;
  colorClass: string;
  datetime: string;
  read?: boolean;
  phone: string;
  email: string;
  creditMemoNumber: number;
  amount: number;
  note: string;
  orderId: number;
}
