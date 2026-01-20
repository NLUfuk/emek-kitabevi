import api from './api';

export interface Book {
  id: string;
  isbn?: string;
  barcode?: string;
  title: string;
  author: string;
  publisher: string;
  category: string;
  currentPrice: number;
  stockQuantity: number;
  minStockLevel: number;
  description?: string;
  createdAt: string;
  updatedAt: string;
  createdBy: string;
  isLowStock: boolean;
}

export interface CreateBookRequest {
  isbn?: string;
  barcode?: string;
  title: string;
  author: string;
  publisher: string;
  category: string;
  currentPrice: number;
  stockQuantity: number;
  minStockLevel: number;
  description?: string;
}

export interface UpdateBookRequest {
  isbn?: string;
  barcode?: string;
  title: string;
  author: string;
  publisher: string;
  category: string;
  currentPrice: number;
  stockQuantity: number;
  minStockLevel: number;
  description?: string;
}

export interface UpdatePriceRequest {
  newPrice: number;
  changeReason?: string;
}

export interface UpdateStockRequest {
  quantity: number;
  movementType: 'In' | 'Out' | 'Adjustment';
  reason: string;
}

export interface BookSearchRequest {
  searchTerm?: string;
  category?: string;
  author?: string;
  lowStockOnly?: boolean;
  pageNumber?: number;
  pageSize?: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface PriceHistory {
  id: string;
  bookId: string;
  bookTitle: string;
  oldPrice: number;
  newPrice: number;
  changedBy: string;
  changeReason?: string;
  changedAt: string;
}

export const bookService = {
  async getAll(request: BookSearchRequest = {}): Promise<PagedResult<Book>> {
    const params = new URLSearchParams();
    if (request.searchTerm) params.append('searchTerm', request.searchTerm);
    if (request.category) params.append('category', request.category);
    if (request.author) params.append('author', request.author);
    if (request.lowStockOnly !== undefined) params.append('lowStockOnly', String(request.lowStockOnly));
    if (request.pageNumber) params.append('pageNumber', String(request.pageNumber));
    if (request.pageSize) params.append('pageSize', String(request.pageSize));

    const response = await api.get<PagedResult<Book>>(`/books?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<Book> {
    const response = await api.get<Book>(`/books/${id}`);
    return response.data;
  },

  async create(data: CreateBookRequest): Promise<Book> {
    const response = await api.post<Book>('/books', data);
    return response.data;
  },

  async update(id: string, data: UpdateBookRequest): Promise<Book> {
    const response = await api.put<Book>(`/books/${id}`, data);
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await api.delete(`/books/${id}`);
  },

  async search(request: BookSearchRequest): Promise<PagedResult<Book>> {
    const params = new URLSearchParams();
    if (request.searchTerm) params.append('searchTerm', request.searchTerm);
    if (request.category) params.append('category', request.category);
    if (request.author) params.append('author', request.author);
    if (request.lowStockOnly !== undefined) params.append('lowStockOnly', String(request.lowStockOnly));
    if (request.pageNumber) params.append('pageNumber', String(request.pageNumber));
    if (request.pageSize) params.append('pageSize', String(request.pageSize));

    const response = await api.get<PagedResult<Book>>(`/books/search?${params.toString()}`);
    return response.data;
  },

  async updatePrice(id: string, data: UpdatePriceRequest): Promise<Book> {
    const response = await api.put<Book>(`/books/${id}/price`, data);
    return response.data;
  },

  async updateStock(id: string, data: UpdateStockRequest): Promise<Book> {
    const response = await api.put<Book>(`/books/${id}/stock`, data);
    return response.data;
  },

  async getPriceHistory(id: string): Promise<PriceHistory[]> {
    const response = await api.get<PriceHistory[]>(`/books/${id}/price-history`);
    return response.data;
  },

  async getLowStockBooks(): Promise<Book[]> {
    const response = await api.get<Book[]>('/books/low-stock');
    return response.data;
  },
};
