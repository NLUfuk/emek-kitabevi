import { Card, CardContent, Typography, Box, Chip, IconButton } from '@mui/material';
import { Book } from '../services/bookService';
import { Edit, Delete } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

interface BookCardProps {
  book: Book;
  onEdit?: (id: string) => void;
  onDelete?: (id: string) => void;
}

export const BookCard = ({ book, onEdit, onDelete }: BookCardProps) => {
  const navigate = useNavigate();

  return (
    <Card
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        cursor: 'pointer',
        '&:hover': {
          boxShadow: 4,
        },
      }}
      onClick={() => navigate(`/books/${book.id}`)}
    >
      <CardContent sx={{ flexGrow: 1 }}>
        <Box display="flex" justifyContent="space-between" alignItems="start" mb={1}>
          <Typography variant="h6" component="h2" noWrap>
            {book.title}
          </Typography>
          {book.isLowStock && (
            <Chip label="Düşük Stok" color="warning" size="small" />
          )}
        </Box>

        <Typography variant="body2" color="text.secondary" gutterBottom>
          {book.author}
        </Typography>

        <Box mt={2}>
          <Typography variant="body2">
            <strong>Yayınevi:</strong> {book.publisher}
          </Typography>
          <Typography variant="body2">
            <strong>Kategori:</strong> {book.category}
          </Typography>
          {book.isbn && (
            <Typography variant="body2">
              <strong>ISBN:</strong> {book.isbn}
            </Typography>
          )}
          {book.barcode && (
            <Typography variant="body2">
              <strong>Barkod:</strong> {book.barcode}
            </Typography>
          )}
        </Box>

        <Box mt={2} display="flex" justifyContent="space-between" alignItems="center">
          <Box>
            <Typography variant="h6" color="primary">
              {book.currentPrice.toFixed(2)} ₺
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Stok: {book.stockQuantity}
            </Typography>
          </Box>
          <Box>
            {onEdit && (
              <IconButton
                size="small"
                onClick={(e) => {
                  e.stopPropagation();
                  onEdit(book.id);
                }}
                color="primary"
              >
                <Edit />
              </IconButton>
            )}
            {onDelete && (
              <IconButton
                size="small"
                onClick={(e) => {
                  e.stopPropagation();
                  onDelete(book.id);
                }}
                color="error"
              >
                <Delete />
              </IconButton>
            )}
          </Box>
        </Box>
      </CardContent>
    </Card>
  );
};
