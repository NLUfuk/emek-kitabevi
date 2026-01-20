import api from './api';
import { PagedResult } from './bookService';

export interface Transaction {
  id: string;
  transactionType: string;
  bookId: string;
  bookTitle: string;
  quantity: number;
  unitPrice: number;
  totalAmount: number;
  transactionDate: string;
  createdBy: string;
  notes?: string;
}

export interface CreateSaleRequest {
  bookId: string;
  quantity: number;
  unitPrice: number;
  notes?: string;
}

export interface CreatePurchaseRequest {
  bookId: string;
  quantity: number;
  unitPrice: number;
  notes?: string;
}

export interface CreateReturnRequest {
  bookId: string;
  quantity: number;
  unitPrice: number;
  notes?: string;
}

export interface TransactionSearchRequest {
  bookId?: string;
  transactionType?: string;
  startDate?: string;
  endDate?: string;
  pageNumber?: number;
  pageSize?: number;
}

export const transactionService = {
  async getAll(request: TransactionSearchRequest = {}): Promise<PagedResult<Transaction>> {
    const params = new URLSearchParams();
    if (request.bookId) params.append('bookId', request.bookId);
    if (request.transactionType) params.append('transactionType', request.transactionType);
    if (request.startDate) params.append('startDate', request.startDate);
    if (request.endDate) params.append('endDate', request.endDate);
    if (request.pageNumber) params.append('pageNumber', String(request.pageNumber));
    if (request.pageSize) params.append('pageSize', String(request.pageSize));

    const response = await api.get<PagedResult<Transaction>>(`/transactions?${params.toString()}`);
    return response.data;
  },

  async getById(id: string): Promise<Transaction> {
    const response = await api.get<Transaction>(`/transactions/${id}`);
    return response.data;
  },

  async createSale(data: CreateSaleRequest): Promise<Transaction> {
    const response = await api.post<Transaction>('/transactions/sale', data);
    return response.data;
  },

  async createPurchase(data: CreatePurchaseRequest): Promise<Transaction> {
    const response = await api.post<Transaction>('/transactions/purchase', data);
    return response.data;
  },

  async createReturn(data: CreateReturnRequest): Promise<Transaction> {
    const response = await api.post<Transaction>('/transactions/return', data);
    return response.data;
  },
};
