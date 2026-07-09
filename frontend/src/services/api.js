import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5139/api';
const API_HOST = API_BASE_URL.endsWith('/api') ? API_BASE_URL.slice(0, -4) : API_BASE_URL;

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Attach token if available
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Global error logging and logout on unauthorized
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response) {
      // Token expired or invalid
      if (error.response.status === 401) {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        // Optional redirect to login:
        if (window.location.pathname !== '/login' && window.location.pathname !== '/register') {
          window.location.href = '/login';
        }
      }
      
      // Extract formatted backend validation errors or message
      const data = error.response.data;
      if (data && data.errors) {
        return Promise.reject({
          status: error.response.status,
          validationErrors: data.errors,
          message: data.detail || 'Validation error occurred.',
        });
      }
      
      return Promise.reject({
        status: error.response.status,
        message: data.detail || data.message || 'An error occurred on the server.',
      });
    }
    
    return Promise.reject({
      status: 0,
      message: 'Network error or server unreachable.',
    });
  }
);

export default api;
export { API_BASE_URL, API_HOST };
