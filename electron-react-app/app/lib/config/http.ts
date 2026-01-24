
import axios, { AxiosInstance, InternalAxiosRequestConfig } from 'axios';
import { toast } from 'sonner';

class HttpService {
  private axiosInstance: AxiosInstance;

  constructor() {
    this.axiosInstance = axios.create({
      baseURL: 'http://localhost:5014',
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    // Request interceptor - Add auth token
    this.axiosInstance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = localStorage.getItem('accessToken');
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
      },
      (error) => {
        return Promise.reject(error);
      }
    );


    this.axiosInstance.interceptors.response.use(
      (response) => response,
      (error) => {
        const message = error.response?.data?.message || 'حدث خطأ غير متوقع';
        const errors = error.response?.data?.errors;

        // Handle 401 Unauthorized
        if (error.response?.status === 401) {
          // Clear auth data
          localStorage.removeItem('accessToken');
          localStorage.removeItem('refreshToken');
          localStorage.removeItem('user');

          // Redirect to login
          if (typeof window !== 'undefined') {
            window.location.href = '/login';
          }
        }

        // Show error toast
        if (errors && Array.isArray(errors) && errors.length > 0) {
          // If we have specific validation errors, show them
          errors.forEach((err: string) => toast.error(err));
        } else {
          // Show generic error message
          toast.error(message);
        }

        return Promise.reject(error);
      }
    );
  }

  public getAxiosInstance(): AxiosInstance {
    return this.axiosInstance;
  }
}

export const httpService = new HttpService();

