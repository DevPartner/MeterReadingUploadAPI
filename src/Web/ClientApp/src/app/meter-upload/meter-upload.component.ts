import { Component } from '@angular/core';
import { MeterReadingItemsClient, UploadResult, FileParameter } from '../web-api-client';

@Component({
  selector: 'app-meter-upload',
  templateUrl: './meter-upload.component.html',
  styleUrls: ['./meter-upload.component.scss']
})
export class MeterUploadComponent {
  selectedFile?: File;
  error?: string;
  response?: UploadResult;

  constructor(private meterReadingItemsClient: MeterReadingItemsClient) { }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input?.files?.length) {
      this.selectedFile = input.files[0];
      this.error = undefined;
      this.response = undefined;
    }
  }

  uploadFile(): void {
    if (!this.selectedFile) {
      this.error = 'No file selected.';
      return;
    }

    const fileParam: FileParameter = {
      data: this.selectedFile,
      fileName: this.selectedFile.name
    };

    this.meterReadingItemsClient.createMeterReadingItems(fileParam).subscribe({
      next: (res: UploadResult) => {
        this.response = res;
        this.error = undefined;
      },
      error: (err) => {
        this.response = undefined;
        this.error = 'Failed to upload file. Please try again.';
        console.error('Upload error:', err);
      }
    });
  }
}
