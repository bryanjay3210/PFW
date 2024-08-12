import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'vex-pop-up',
  templateUrl: './pop-up.component.html',
  styleUrls: ['./pop-up.component.scss']
})
export class PopUpComponent implements OnInit {
  title = "";
  message = "";

  constructor(private dialogRef: MatDialogRef<PopUpComponent>, @Inject(MAT_DIALOG_DATA) public data: {title: string, message: string}) {
    this.title = this.data.title;
    this.message = this.data.message;
  }

  ngOnInit(): void {}

  close(answer: string) {
    return this.dialogRef.close(answer);
  }
}