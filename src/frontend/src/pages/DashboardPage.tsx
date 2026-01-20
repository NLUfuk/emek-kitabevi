import { Container, Typography, Box, Button, Paper, Grid } from '@mui/material';
import { Book, MenuBook } from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export const DashboardPage = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <Container>
      <Box sx={{ mt: 4 }}>
        <Paper sx={{ p: 3 }}>
          <Typography variant="h4" gutterBottom>
            Hoş Geldiniz, {user?.fullName}!
          </Typography>
          <Typography variant="body1" color="textSecondary" gutterBottom>
            Kullanıcı Adı: {user?.username}
          </Typography>
          <Typography variant="body1" color="textSecondary" gutterBottom>
            E-posta: {user?.email}
          </Typography>
          <Typography variant="body1" color="textSecondary" gutterBottom>
            Rol: {user?.role}
          </Typography>
        </Paper>

        <Grid container spacing={3} sx={{ mt: 2 }}>
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 3, textAlign: 'center', cursor: 'pointer', '&:hover': { boxShadow: 4 } }}
              onClick={() => navigate('/books')}>
              <MenuBook sx={{ fontSize: 48, color: 'primary.main', mb: 2 }} />
              <Typography variant="h6">Kitaplar</Typography>
              <Typography variant="body2" color="text.secondary">
                Kitap listesini görüntüle ve yönet
              </Typography>
            </Paper>
          </Grid>
          <Grid item xs={12} md={6}>
            <Paper sx={{ p: 3, textAlign: 'center', cursor: 'pointer', '&:hover': { boxShadow: 4 } }}
              onClick={() => navigate('/books/new')}>
              <Book sx={{ fontSize: 48, color: 'primary.main', mb: 2 }} />
              <Typography variant="h6">Yeni Kitap</Typography>
              <Typography variant="body2" color="text.secondary">
                Yeni kitap ekle
              </Typography>
            </Paper>
          </Grid>
        </Grid>

        <Box sx={{ mt: 3, textAlign: 'center' }}>
          <Button
            variant="outlined"
            color="error"
            onClick={handleLogout}
          >
            Çıkış Yap
          </Button>
        </Box>
      </Box>
    </Container>
  );
};
