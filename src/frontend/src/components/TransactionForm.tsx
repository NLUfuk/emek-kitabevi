import { useState } from 'react';
import { TextField, Grid, Button, Box, Autocomplete, Alert } from '@mui/material';
import { Book } from '../services/bookService';

interface TransactionFormProps {
  books: Book[];
  onSubmit: (data: { bookId: string; quantity: number; unitPrice: number; notes?: string }) => void;
  onCancel: () => void;
  isLoading?: boolean;
  transactionType: 'Sale' | 'Purchase' | 'Return';
}

export const TransactionForm = ({
  books,
  onSubmit,
  onCancel,
  isLoading = false,
  transactionType,
}: TransactionFormProps) => {
  const [selectedBook, setSelectedBook] = useState<Book | null>(null);
  const [quantity, setQuantity] = useState('');
  const [unitPrice, setUnitPrice] = useState('');
  const [notes, setNotes] = useState('');

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    
    if (!selectedBook) {
      return;
    }

    const data = {
      bookId: selectedBook.id,
      quantity: parseInt(quantity),
      unitPrice: parseFloat(unitPrice) || selectedBook.currentPrice,
      notes: notes || undefined,
    };
    onSubmit(data);
  };

  const handleBookChange = (book: Book | null) => {
    setSelectedBook(book);
    if (book) {
      setUnitPrice(book.currentPrice.toString());
    }
  };

  const getTitle = () => {
    switch (transactionType) {
      case 'Sale':
        return 'Satış İşlemi';
      case 'Purchase':
        return 'Alış İşlemi';
      case 'Return':
        return 'İade İşlemi';
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Grid container spacing={3}>
        <Grid item xs={12}>
          <Autocomplete
            options={books}
            getOptionLabel={(option) => `${option.title} - ${option.author} (Stok: ${option.stockQuantity})`}
            value={selectedBook}
            onChange={(_, value) => handleBookChange(value)}
            renderInput={(params) => (
              <TextField
                {...params}
                label="Kitap *"
                required
                helperText={transactionType === 'Sale' ? 'Satış için yeterli stok kontrolü yapılacaktır' : ''}
              />
            )}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Miktar *"
            type="number"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            inputProps={{ min: '1' }}
            required
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Birim Fiyat (₺) *"
            type="number"
            value={unitPrice}
            onChange={(e) => setUnitPrice(e.target.value)}
            inputProps={{ step: '0.01', min: '0.01' }}
            required
            helperText="Boş bırakılırsa kitabın mevcut fiyatı kullanılır"
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Notlar"
            multiline
            rows={3}
            value={notes}
            onChange={(e) => setNotes(e.target.value)}
          />
        </Grid>
        {transactionType === 'Sale' && (
          <Grid item xs={12}>
            <Alert severity="info">
              Satış işlemi yapıldığında stok otomatik olarak azaltılacaktır.
            </Alert>
          </Grid>
        )}
        {transactionType === 'Purchase' && (
          <Grid item xs={12}>
            <Alert severity="info">
              Alış işlemi yapıldığında stok otomatik olarak artırılacaktır.
            </Alert>
          </Grid>
        )}
        {transactionType === 'Return' && (
          <Grid item xs={12}>
            <Alert severity="info">
              İade işlemi yapıldığında stok otomatik olarak artırılacaktır.
            </Alert>
          </Grid>
        )}
        <Grid item xs={12}>
          <Box display="flex" gap={2} justifyContent="flex-end">
            <Button onClick={onCancel} disabled={isLoading}>
              İptal
            </Button>
            <Button type="submit" variant="contained" disabled={isLoading}>
              {isLoading ? 'İşleniyor...' : getTitle()}
            </Button>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
};
