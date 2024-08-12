import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'vex-print-invoice',
  templateUrl: './print-invoice.component.html',
  styleUrls: ['./print-invoice.component.scss']
})
export class PrintInvoiceComponent implements OnInit {
  data: any;
  dataSource = {
    orderNumber: 'ORN01354',
    invoiceNumber: 'INV01354',
    purchaseOrderNumber:'464589123',
    invoiceDate: new Date().toLocaleString(),
    address: '3383 OLIVE AVE, SIGNAL HILL CA 90755',
    printDate: new Date().toLocaleString(),
    phoneNumber: '310-956-4667',
    website: 'PERFECTFITWEST.COM',
    page: 1,
    pages: 1,
    soldTo: 'NOEL & CO.',
    soldToAddress: '2612 E ADAMS ST, CARSON CA 90810',
    shipTo: 'NOEL & CO.',
    shipToAddress: '2612 E ADAMS ST, CARSON CA 90810',
    accountNumber: '66465',
    customerPhoneNumber: '5626375489',
    customerTerms: 'NET 30 DAYS',
    soldBy: 'JOMARK',
    notes:'CALL CUSTOMER AHEAD OF TIME BEFORE DELIVERY',
    orderedBy:'NELSON',
    orderedByPhone:'8879564123',
    zone: '9',
    vendorCode: 'PROA-CAR-STOCK-PCH',
    deliveryDate: new Date().toLocaleString(),
    subTotal: '485.63',
    tax: '15.00',
    total: '500.63',
    staticText1: '20% RESTOCKING FEE AFTER 10 DAYS, NO RETURNS AFTER 30 DAYS',
    staticText2: '50% RESTOCKING FEE FOR NO BAG/BOX ITEMS',
    totalQuantity: 20,
    lineItems: [
      { quantity: 1, partNumber: 'THIRTEEN12345', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: '3ST76010006P2', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
      { quantity: 1, partNumber: 'QT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '25.25', price: '15.25', extPrice: '15.25'},
      { quantity: 1, partNumber: 'AT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '135.35', price: '83.00', extPrice: '83.00'},
      { quantity: 1, partNumber: 'RT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: 'ST76010006P23', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
      { quantity: 1, partNumber: '3QT76010006P3', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '25.25', price: '15.25', extPrice: '15.25'},
      { quantity: 1, partNumber: 'AT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '135.35', price: '83.00', extPrice: '83.00'},
      { quantity: 1, partNumber: 'RT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: '3ST76010006P2', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
      { quantity: 1, partNumber: 'RT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: '3ST76010006P2', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
      { quantity: 1, partNumber: 'QT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '25.25', price: '15.25', extPrice: '15.25'},
      { quantity: 1, partNumber: 'AT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '135.35', price: '83.00', extPrice: '83.00'},
      { quantity: 1, partNumber: 'RT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: '3ST76010006P2', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
      { quantity: 1, partNumber: 'QT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '25.25', price: '15.25', extPrice: '15.25'},
      { quantity: 1, partNumber: 'AT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '135.35', price: '83.00', extPrice: '83.00'},
      { quantity: 1, partNumber: 'RT76010006P', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '235.55', price: '85.55', extPrice: '85.55'},
      { quantity: 1, partNumber: '3ST76010006P2', description:'2016-2018 PRIUS 16-18 REAR BUMPER COVER, PRIMED, W/ PARKING AID SENSOR HOLES, K100014557, 01-02-AO1, STOCK', lprice: '250.00', price: '155.50', extPrice: '155.50'},
    ]
  }
  
  constructor() { }

  ngOnInit(): void {
    setTimeout( () => { this.data = this.dataSource; }, 1000 );
  }

  printInvoice() {
    window.print();
  }
}
