import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Container,
  Grid,
  Typography,
  Button,
  Box,
  TextField,
  MenuItem,
  Pagination,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { BookCard } from '../components/BookCard';
import { SearchBar } from '../components/SearchBar';
import { bookService, Book, BookSearchRequest } from '../services/bookService';

export const BooksPage = () => {
  const navigate = useNavigate();
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [category, setCategory] = useState('');
  const [lowStockOnly, setLowStockOnly] = useState(false);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(12);
  const [totalPages, setTotalPages] = useState(1);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [bookToDelete, setBookToDelete] = useState<string | null>(null);

  const loadBooks = async () => {
    setLoading(true);
    setError('');
    try {
      const request: BookSearchRequest = {
        searchTerm: searchTerm || undefined,
        category: category || undefined,
        lowStockOnly: lowStockOnly || undefined,
        pageNumber,
        pageSize,
      };
      const result = await bookService.getAll(request);
      setBooks(result.items);
      setTotalPages(result.totalPages);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kitaplar yüklenirken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadBooks();
  }, [pageNumber, lowStockOnly]);

  const handleSearch = () => {
    setPageNumber(1);
    loadBooks();
  };

  const handleDelete = async () => {
    if (!bookToDelete) return;

    try {
      await bookService.delete(bookToDelete);
      setDeleteDialogOpen(false);
      setBookToDelete(null);
      loadBooks();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kitap silinirken bir hata oluştu');
    }
  };

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Kitaplar</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={() => navigate('/books/new')}
        >
          Yeni Kitap
        </Button>
      </Box>

      {error && (
        <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
          {error}
        </Alert>
      )}

      <SearchBar
        searchTerm={searchTerm}
        onSearchChange={setSearchTerm}
        onSearch={handleSearch}
      />

      <Box display="flex" gap={2} mb={3}>
        <TextField
          select
          label="Kategori"
          value={category}
          onChange={(e) => setCategory(e.target.value)}
          sx={{ minWidth: 200 }}
        >
          <MenuItem value="">Tümü</MenuItem>
          <MenuItem value="Roman">Roman</MenuItem>
          <MenuItem value="Hikaye">Hikaye</MenuItem>
          <MenuItem value="Şiir">Şiir</MenuItem>
          <MenuItem value="Tarih">Tarih</MenuItem>
          <MenuItem value="Bilim">Bilim</MenuItem>
          <MenuItem value="Eğitim">Eğitim</MenuItem>
        </TextField>
        <Button
          variant={lowStockOnly ? 'contained' : 'outlined'}
          onClick={() => {
            setLowStockOnly(!lowStockOnly);
            setPageNumber(1);
          }}
        >
          Sadece Düşük Stoklu
        </Button>
      </Box>

      {loading ? (
        <Typography>Yükleniyor...</Typography>
      ) : books.length === 0 ? (
        <Alert severity="info">Kitap bulunamadı</Alert>
      ) : (
        <>
          <Grid container spacing={3}>
            {books.map((book) => (
              <Grid item xs={12} sm={6} md={4} lg={3} key={book.id}>
                <BookCard
                  book={book}
                  onEdit={(id) => navigate(`/books/${id}/edit`)}
                  onDelete={(id) => {
                    setBookToDelete(id);
                    setDeleteDialogOpen(true);
                  }}
                />
              </Grid>
            ))}
          </Grid>

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

      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Kitap Sil</DialogTitle>
        <DialogContent>
          <DialogContentText>
            Bu kitabı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz.
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>İptal</Button>
          <Button onClick={handleDelete} color="error" variant="contained">
            Sil
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
};
