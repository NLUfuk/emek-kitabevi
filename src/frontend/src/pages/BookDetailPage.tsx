import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Container,
  Paper,
  Typography,
  Box,
  Button,
  Grid,
  Chip,
  Divider,
  Tab,
  Tabs,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  MenuItem,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import { Edit, ArrowBack } from '@mui/icons-material';
import { BookForm } from '../components/BookForm';
import { bookService, Book, PriceHistory, UpdatePriceRequest, UpdateStockRequest } from '../services/bookService';
import { format } from 'date-fns';

export const BookDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [book, setBook] = useState<Book | null>(null);
  const [priceHistory, setPriceHistory] = useState<PriceHistory[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [tabValue, setTabValue] = useState(0);
  const [editMode, setEditMode] = useState(false);
  const [priceDialogOpen, setPriceDialogOpen] = useState(false);
  const [stockDialogOpen, setStockDialogOpen] = useState(false);
  const [newPrice, setNewPrice] = useState('');
  const [priceReason, setPriceReason] = useState('');
  const [stockQuantity, setStockQuantity] = useState('');
  const [stockType, setStockType] = useState<'In' | 'Out' | 'Adjustment'>('In');
  const [stockReason, setStockReason] = useState('');

  useEffect(() => {
    if (id) {
      loadBook();
      loadPriceHistory();
    }
  }, [id]);

  const loadBook = async () => {
    if (!id) return;
    setLoading(true);
    try {
      const data = await bookService.getById(id);
      setBook(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kitap yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  const loadPriceHistory = async () => {
    if (!id) return;
    try {
      const history = await bookService.getPriceHistory(id);
      setPriceHistory(history);
    } catch (err) {
      console.error('Fiyat geçmişi yüklenirken hata:', err);
    }
  };

  const handleUpdatePrice = async () => {
    if (!id || !newPrice) return;
    try {
      const request: UpdatePriceRequest = {
        newPrice: parseFloat(newPrice),
        changeReason: priceReason || undefined,
      };
      await bookService.updatePrice(id, request);
      setPriceDialogOpen(false);
      setNewPrice('');
      setPriceReason('');
      loadBook();
      loadPriceHistory();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Fiyat güncellenirken bir hata oluştu');
    }
  };

  const handleUpdateStock = async () => {
    if (!id || !stockQuantity || !stockReason) return;
    try {
      const request: UpdateStockRequest = {
        quantity: parseInt(stockQuantity),
        movementType: stockType,
        reason: stockReason,
      };
      await bookService.updateStock(id, request);
      setStockDialogOpen(false);
      setStockQuantity('');
      setStockReason('');
      loadBook();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Stok güncellenirken bir hata oluştu');
    }
  };

  const handleUpdateBook = async (data: any) => {
    if (!id) return;
    setLoading(true);
    try {
      await bookService.update(id, data);
      setEditMode(false);
      loadBook();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kitap güncellenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  if (loading && !book) {
    return <Container>Yükleniyor...</Container>;
  }

  if (!book) {
    return <Container>Kitap bulunamadı</Container>;
  }

  return (
    <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
      <Button startIcon={<ArrowBack />} onClick={() => navigate('/books')} sx={{ mb: 2 }}>
        Geri
      </Button>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <Paper sx={{ p: 4 }}>
        {!editMode ? (
          <>
            <Box display="flex" justifyContent="space-between" alignItems="start" mb={3}>
              <Box>
                <Typography variant="h4" gutterBottom>
                  {book.title}
                </Typography>
                <Typography variant="h6" color="text.secondary" gutterBottom>
                  {book.author}
                </Typography>
                {book.isLowStock && (
                  <Chip label="Düşük Stok" color="warning" sx={{ mt: 1 }} />
                )}
              </Box>
              <Button
                variant="contained"
                startIcon={<Edit />}
                onClick={() => setEditMode(true)}
              >
                Düzenle
              </Button>
            </Box>

            <Divider sx={{ my: 3 }} />

            <Grid container spacing={3}>
              <Grid item xs={12} md={6}>
                <Typography variant="body2" color="text.secondary">Yayınevi</Typography>
                <Typography variant="body1" gutterBottom>{book.publisher}</Typography>
              </Grid>
              <Grid item xs={12} md={6}>
                <Typography variant="body2" color="text.secondary">Kategori</Typography>
                <Typography variant="body1" gutterBottom>{book.category}</Typography>
              </Grid>
              {book.isbn && (
                <Grid item xs={12} md={6}>
                  <Typography variant="body2" color="text.secondary">ISBN</Typography>
                  <Typography variant="body1" gutterBottom>{book.isbn}</Typography>
                </Grid>
              )}
              {book.barcode && (
                <Grid item xs={12} md={6}>
                  <Typography variant="body2" color="text.secondary">Barkod</Typography>
                  <Typography variant="body1" gutterBottom>{book.barcode}</Typography>
                </Grid>
              )}
              <Grid item xs={12} md={4}>
                <Typography variant="body2" color="text.secondary">Fiyat</Typography>
                <Typography variant="h5" color="primary" gutterBottom>
                  {book.currentPrice.toFixed(2)} ₺
                </Typography>
                <Button size="small" onClick={() => setPriceDialogOpen(true)}>
                  Fiyat Güncelle
                </Button>
              </Grid>
              <Grid item xs={12} md={4}>
                <Typography variant="body2" color="text.secondary">Stok Miktarı</Typography>
                <Typography variant="h5" gutterBottom>{book.stockQuantity}</Typography>
                <Button size="small" onClick={() => setStockDialogOpen(true)}>
                  Stok Güncelle
                </Button>
              </Grid>
              <Grid item xs={12} md={4}>
                <Typography variant="body2" color="text.secondary">Minimum Stok</Typography>
                <Typography variant="body1" gutterBottom>{book.minStockLevel}</Typography>
              </Grid>
              {book.description && (
                <Grid item xs={12}>
                  <Typography variant="body2" color="text.secondary">Açıklama</Typography>
                  <Typography variant="body1">{book.description}</Typography>
                </Grid>
              )}
            </Grid>
          </>
        ) : (
          <>
            <Typography variant="h4" gutterBottom>
              Kitap Düzenle
            </Typography>
            <BookForm
              initialData={book}
              onSubmit={handleUpdateBook}
              onCancel={() => setEditMode(false)}
              isLoading={loading}
              isEdit
            />
          </>
        )}
      </Paper>

      <Paper sx={{ mt: 3, p: 2 }}>
        <Tabs value={tabValue} onChange={(_, v) => setTabValue(v)}>
          <Tab label="Fiyat Geçmişi" />
        </Tabs>
        {tabValue === 0 && (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Eski Fiyat</TableCell>
                  <TableCell>Yeni Fiyat</TableCell>
                  <TableCell>Değiştiren</TableCell>
                  <TableCell>Neden</TableCell>
                  <TableCell>Tarih</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {priceHistory.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={5} align="center">
                      Fiyat geçmişi bulunamadı
                    </TableCell>
                  </TableRow>
                ) : (
                  priceHistory.map((history) => (
                    <TableRow key={history.id}>
                      <TableCell>{history.oldPrice.toFixed(2)} ₺</TableCell>
                      <TableCell>{history.newPrice.toFixed(2)} ₺</TableCell>
                      <TableCell>{history.changedBy}</TableCell>
                      <TableCell>{history.changeReason || '-'}</TableCell>
                      <TableCell>
                        {format(new Date(history.changedAt), 'dd MMM yyyy HH:mm')}
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>

      {/* Fiyat Güncelleme Dialog */}
      <Dialog open={priceDialogOpen} onClose={() => setPriceDialogOpen(false)}>
        <DialogTitle>Fiyat Güncelle</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            label="Yeni Fiyat (₺)"
            type="number"
            value={newPrice}
            onChange={(e) => setNewPrice(e.target.value)}
            inputProps={{ step: '0.01', min: '0.01' }}
            sx={{ mt: 2 }}
          />
          <TextField
            fullWidth
            label="Değişiklik Nedeni"
            value={priceReason}
            onChange={(e) => setPriceReason(e.target.value)}
            sx={{ mt: 2 }}
            multiline
            rows={3}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPriceDialogOpen(false)}>İptal</Button>
          <Button onClick={handleUpdatePrice} variant="contained" disabled={!newPrice}>
            Güncelle
          </Button>
        </DialogActions>
      </Dialog>

      {/* Stok Güncelleme Dialog */}
      <Dialog open={stockDialogOpen} onClose={() => setStockDialogOpen(false)}>
        <DialogTitle>Stok Güncelle</DialogTitle>
        <DialogContent>
          <TextField
            fullWidth
            select
            label="Hareket Tipi"
            value={stockType}
            onChange={(e) => setStockType(e.target.value as any)}
            sx={{ mt: 2 }}
          >
            <MenuItem value="In">Giriş</MenuItem>
            <MenuItem value="Out">Çıkış</MenuItem>
            <MenuItem value="Adjustment">Düzeltme</MenuItem>
          </TextField>
          <TextField
            fullWidth
            label="Miktar"
            type="number"
            value={stockQuantity}
            onChange={(e) => setStockQuantity(e.target.value)}
            inputProps={{ min: '1' }}
            sx={{ mt: 2 }}
          />
          <TextField
            fullWidth
            label="Neden *"
            value={stockReason}
            onChange={(e) => setStockReason(e.target.value)}
            sx={{ mt: 2 }}
            multiline
            rows={3}
            required
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setStockDialogOpen(false)}>İptal</Button>
          <Button onClick={handleUpdateStock} variant="contained" disabled={!stockQuantity || !stockReason}>
            Güncelle
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};
