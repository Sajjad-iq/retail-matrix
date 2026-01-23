import axios, { AxiosInstance } from 'axios';

class HttpService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: typeof window === 'undefined'
        ? (process.env.NEXT_PUBLIC_API_URL || 'http://localhost:3000/api')
        : '/api',
      timeout: 30000,
      withCredentials: true,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  public getAxiosInstance(): AxiosInstance {
    return this.axiosInstance;
  }
}

export const httpService = new HttpService();
