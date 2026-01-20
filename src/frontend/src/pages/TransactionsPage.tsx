import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Typography,
  Button,
  Box,
  TextField,
  MenuItem,
  Pagination,
  Alert,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Tabs,
  Tab,
} from '@mui/material';
import { Add, ShoppingCart, ShoppingBag, Undo } from '@mui/icons-material';
import { TransactionForm } from '../components/TransactionForm';
import { transactionService, Transaction, TransactionSearchRequest } from '../services/transactionService';
import { bookService, Book } from '../services/bookService';
import { format } from 'date-fns';

export const TransactionsPage = () => {
  const navigate = useNavigate();
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [transactionType, setTransactionType] = useState<string>('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [dialogOpen, setDialogOpen] = useState(false);
  const [dialogType, setDialogType] = useState<'Sale' | 'Purchase' | 'Return'>('Sale');
  const [tabValue, setTabValue] = useState(0);

  useEffect(() => {
    loadTransactions();
    loadBooks();
  }, [pageNumber, transactionType]);

  const loadTransactions = async () => {
    setLoading(true);
    setError('');
    try {
      const request: TransactionSearchRequest = {
        transactionType: transactionType || undefined,
        startDate: startDate || undefined,
        endDate: endDate || undefined,
        pageNumber,
        pageSize,
      };
      const result = await transactionService.getAll(request);
      setTransactions(result.items);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      setError(err.response?.data?.message || 'İşlemler yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const loadBooks = async () => {
    try {
      const result = await bookService.getAll({ pageSize: 1000 });
      setBooks(result.items);
    } catch (err) {
      console.error('Kitaplar yüklenirken hata:', err);
    }
  };

  const handleSearch = () => {
    setPageNumber(1);
    loadTransactions();
  };

  const handleOpenDialog = (type: 'Sale' | 'Purchase' | 'Return') => {
    setDialogType(type);
    setDialogOpen(true);
  };

  const handleSubmitTransaction = async (data: { bookId: string; quantity: number; unitPrice: number; notes?: string }) => {
    setLoading(true);
    setError('');
    try {
      switch (dialogType) {
        case 'Sale':
          await transactionService.createSale(data);
          break;
        case 'Purchase':
          await transactionService.createPurchase(data);
          break;
        case 'Return':
          await transactionService.createReturn(data);
          break;
      }
      setDialogOpen(false);
      loadTransactions();
      loadBooks(); // Stok güncellemesi için kitapları yeniden yükle
    } catch (err: any) {
      setError(err.response?.data?.message || 'İşlem oluşturulurken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const getTransactionTypeColor = (type: string) => {
    switch (type) {
      case 'Sale':
        return 'success';
      case 'Purchase':
        return 'info';
      case 'Return':
        return 'warning';
      default:
        return 'default';
    }
  };

  const getTransactionTypeLabel = (type: string) => {
    switch (type) {
      case 'Sale':
        return 'Satış';
      case 'Purchase':
        return 'Alış';
      case 'Return':
        return 'İade';
      default:
        return type;
    }
  };

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">İşlemler</Typography>
        <Box display="flex" gap={1}>
          <Button
            variant="contained"
            color="success"
            startIcon={<ShoppingCart />}
            onClick={() => handleOpenDialog('Sale')}
          >
            Satış
          </Button>
          <Button
            variant="contained"
            color="info"
            startIcon={<ShoppingBag />}
            onClick={() => handleOpenDialog('Purchase')}
          >
            Alış
          </Button>
          <Button
            variant="contained"
            color="warning"
            startIcon={<Undo />}
            onClick={() => handleOpenDialog('Return')}
          >
            İade
          </Button>
        </Box>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 3, mb: 3 }}>
        <Box display="flex" gap={2} flexWrap="wrap">
          <TextField
            select
            label="İşlem Tipi"
            value={transactionType}
            onChange={(e) => setTransactionType(e.target.value)}
            sx={{ minWidth: 200 }}
          >
            <MenuItem value="">Tümü</MenuItem>
            <MenuItem value="Sale">Satış</MenuItem>
            <MenuItem value="Purchase">Alış</MenuItem>
            <MenuItem value="Return">İade</MenuItem>
          </TextField>
          <TextField
            label="Başlangıç Tarihi"
            type="date"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
          <TextField
            label="Bitiş Tarihi"
            type="date"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
            InputLabelProps={{ shrink: true }}
          />
          <Button variant="outlined" onClick={handleSearch}>
            Filtrele
          </Button>
        </Box>
      </Paper>

      {loading ? (
        <Typography>Yükleniyor...</Typography>
      ) : transactions.length === 0 ? (
        <Alert severity="info">İşlem bulunamadı</Alert>
      ) : (
        <>
          <TableContainer component={Paper}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Tarih</TableCell>
                  <TableCell>Tip</TableCell>
                  <TableCell>Kitap</TableCell>
                  <TableCell>Miktar</TableCell>
                  <TableCell>Birim Fiyat</TableCell>
                  <TableCell>Toplam</TableCell>
                  <TableCell>İşlemi Yapan</TableCell>
                  <TableCell>Notlar</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {transactions.map((transaction) => (
                  <TableRow key={transaction.id}>
                    <TableCell>
                      {format(new Date(transaction.transactionDate), 'dd MMM yyyy HH:mm')}
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={getTransactionTypeLabel(transaction.transactionType)}
                        color={getTransactionTypeColor(transaction.transactionType) as any}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      <Button
                        size="small"
                        onClick={() => navigate(`/books/${transaction.bookId}`)}
                      >
                        {transaction.bookTitle}
                      </Button>
                    </TableCell>
                    <TableCell>{transaction.quantity}</TableCell>
                    <TableCell>{transaction.unitPrice.toFixed(2)} ₺</TableCell>
                    <TableCell>
                      <strong>{transaction.totalAmount.toFixed(2)} ₺</strong>
                    </TableCell>
                    <TableCell>{transaction.createdBy}</TableCell>
                    <TableCell>{transaction.notes || '-'}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>

          {totalPages > 1 && (
            <Box display="flex" justifyContent="center" mt={4}>
              <Pagination
                count={totalPages}
                page={pageNumber}
                onChange={(_, page) => setPageNumber(page)}
                color="primary"
              />
            </Box>
          )}
        </>
      )}

      {/* Transaction Dialog */}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="md" fullWidth>
        <DialogTitle>
          {dialogType === 'Sale' && 'Satış İşlemi'}
          {dialogType === 'Purchase' && 'Alış İşlemi'}
          {dialogType === 'Return' && 'İade İşlemi'}
        </DialogTitle>
        <DialogContent>
          <TransactionForm
            books={books}
            onSubmit={handleSubmitTransaction}
            onCancel={() => setDialogOpen(false)}
            isLoading={loading}
            transactionType={dialogType}
          />
        </DialogContent>
      </Dialog>
    </Container>
  );
};
