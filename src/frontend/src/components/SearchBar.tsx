import { TextField, Box, Button } from '@mui/material';
import { Search } from '@mui/icons-material';

interface SearchBarProps {
  searchTerm: string;
  onSearchChange: (value: string) => void;
  onSearch: () => void;
  placeholder?: string;
}

export const SearchBar = ({ searchTerm, onSearchChange, onSearch, placeholder = "ISBN, Barkod veya Kitap Adı ile ara..." }: SearchBarProps) => {
  return (
    <Box display="flex" gap={1} mb={3}>
      <TextField
        fullWidth
        variant="outlined"
        placeholder={placeholder}
        value={searchTerm}
        onChange={(e) => onSearchChange(e.target.value)}
        onKeyPress={(e) => {
          if (e.key === 'Enter') {
            onSearch();
          }
        }}
      />
      <Button
        variant="contained"
        startIcon={<Search />}
        onClick={onSearch}
        sx={{ minWidth: 120 }}
      >
        Ara
      </Button>
    </Box>
  );
};
