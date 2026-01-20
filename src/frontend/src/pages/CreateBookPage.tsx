import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Container, Typography, Paper, Alert } from '@mui/material';
import { BookForm } from '../components/BookForm';
import { bookService, CreateBookRequest } from '../services/bookService';

export const CreateBookPage = () => {
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (data: CreateBookRequest) => {
    setLoading(true);
    setError('');
    try {
      await bookService.create(data);
      navigate('/books');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Kitap oluşturulurken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container maxWidth="md" sx={{ mt: 4, mb: 4 }}>
      <Paper sx={{ p: 4 }}>
        <Typography variant="h4" gutterBottom>
          Yeni Kitap Ekle
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>
            {error}
          </Alert>
        )}

        <BookForm
          onSubmit={handleSubmit}
          onCancel={() => navigate('/books')}
          isLoading={loading}
        />
      </Paper>
    </Container>
  );
};
