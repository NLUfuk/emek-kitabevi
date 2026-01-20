import { TextField, Grid, Button, Box } from '@mui/material';
import { CreateBookRequest, UpdateBookRequest } from '../services/bookService';

interface BookFormProps {
  initialData?: Partial<CreateBookRequest>;
  onSubmit: (data: CreateBookRequest | UpdateBookRequest) => void;
  onCancel: () => void;
  isLoading?: boolean;
  isEdit?: boolean;
}

export const BookForm = ({ initialData, onSubmit, onCancel, isLoading = false, isEdit = false }: BookFormProps) => {
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data: CreateBookRequest = {
      isbn: formData.get('isbn') as string || undefined,
      barcode: formData.get('barcode') as string || undefined,
      title: formData.get('title') as string,
      author: formData.get('author') as string,
      publisher: formData.get('publisher') as string,
      category: formData.get('category') as string,
      currentPrice: parseFloat(formData.get('currentPrice') as string),
      stockQuantity: parseInt(formData.get('stockQuantity') as string),
      minStockLevel: parseInt(formData.get('minStockLevel') as string),
      description: formData.get('description') as string || undefined,
    };
    onSubmit(data);
  };

  return (
    <Box component="form" onSubmit={handleSubmit}>
      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Kitap Adı *"
            name="title"
            defaultValue={initialData?.title}
            required
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Yazar"
            name="author"
            defaultValue={initialData?.author}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Yayınevi"
            name="publisher"
            defaultValue={initialData?.publisher}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Kategori"
            name="category"
            defaultValue={initialData?.category}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="ISBN"
            name="isbn"
            defaultValue={initialData?.isbn}
          />
        </Grid>
        <Grid item xs={12} md={6}>
          <TextField
            fullWidth
            label="Barkod"
            name="barcode"
            defaultValue={initialData?.barcode}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            fullWidth
            label="Fiyat (₺) *"
            name="currentPrice"
            type="number"
            inputProps={{ step: '0.01', min: '0.01' }}
            defaultValue={initialData?.currentPrice || 0}
            required
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            fullWidth
            label="Stok Miktarı"
            name="stockQuantity"
            type="number"
            inputProps={{ min: '0' }}
            defaultValue={initialData?.stockQuantity || 0}
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <TextField
            fullWidth
            label="Minimum Stok Seviyesi"
            name="minStockLevel"
            type="number"
            inputProps={{ min: '0' }}
            defaultValue={initialData?.minStockLevel || 5}
          />
        </Grid>
        <Grid item xs={12}>
          <TextField
            fullWidth
            label="Açıklama"
            name="description"
            multiline
            rows={4}
            defaultValue={initialData?.description}
          />
        </Grid>
        <Grid item xs={12}>
          <Box display="flex" gap={2} justifyContent="flex-end">
            <Button onClick={onCancel} disabled={isLoading}>
              İptal
            </Button>
            <Button type="submit" variant="contained" disabled={isLoading}>
              {isLoading ? 'Kaydediliyor...' : isEdit ? 'Güncelle' : 'Kaydet'}
            </Button>
          </Box>
        </Grid>
      </Grid>
    </Box>
  );
};
