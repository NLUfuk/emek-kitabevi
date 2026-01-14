import { Container, Typography, Box, Button, Paper } from '@mui/material';
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
          <Button
            variant="outlined"
            color="error"
            onClick={handleLogout}
            sx={{ mt: 2 }}
          >
            Çıkış Yap
          </Button>
        </Paper>
      </Box>
    </Container>
  );
};
