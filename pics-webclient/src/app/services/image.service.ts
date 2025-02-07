import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Image } from '../models/image';

@Injectable({
  providedIn: 'root'
})
export class ImageService {
  private apiUrl = 'http://localhost:5272/home'

  constructor(private http: HttpClient) { }

  getImages(offset: number, limit: number) : Observable<Image[]> {
    return this.http.get<Image[]>(`${this.apiUrl}?offset=${offset}&limit=${limit}`,
      {withCredentials: true}
    );
  }
}
