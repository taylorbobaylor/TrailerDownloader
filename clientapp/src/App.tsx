import React, { useState } from 'react';
import { AppBar, Toolbar, Typography, Container, Box, CssBaseline, ThemeProvider, createTheme, Card, CardContent, TextField, Button } from '@mui/material';
import './App.css';

interface Movie {
  id: number;
  title: string;
  trailerUrl: string;
}

interface SearchBarProps {
  onSearch: (searchTerm: string) => void;
}

interface MovieListProps {
  movies: Movie[];
}

const theme = createTheme({
  palette: {
    primary: {
      main: '#556cd6',
    },
    secondary: {
      main: '#19857b',
    },
    error: {
      main: '#ff1744',
    },
    background: {
      default: '#fff',
    },
  },
});

function SearchBar({ onSearch }: SearchBarProps) {
  const [searchTerm, setSearchTerm] = useState('');

  const handleSearch = () => {
    onSearch(searchTerm);
  };

  return (
    <Box sx={{ my: 2 }}>
      <TextField
        fullWidth
        label="Search for a movie"
        variant="outlined"
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        onKeyPress={(e) => {
          if (e.key === 'Enter') {
            handleSearch();
          }
        }}
      />
      <Button variant="contained" sx={{ mt: 2 }} onClick={handleSearch}>
        Search
      </Button>
    </Box>
  );
}

function MovieList({ movies }: MovieListProps) {
  return (
    <Box sx={{ my: 4 }}>
      {movies.map((movie: Movie) => (
        <Card key={movie.id} sx={{ mb: 2 }}>
          <CardContent>
            <Typography variant="h5" component="div">
              {movie.title}
            </Typography>
            <Button variant="contained" sx={{ mt: 2 }} onClick={() => {/* Download trailer logic here */}}>
              Download Trailer
            </Button>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}

function App() {
  const [movies, setMovies] = useState<Movie[]>([
    // Placeholder for initial movie data
    { id: 1, title: 'Movie 1', trailerUrl: '#' },
    { id: 2, title: 'Movie 2', trailerUrl: '#' },
  ]);

  const handleSearch = (searchTerm: string) => {
    // Logic to search for movies and update state
    console.log('Search for:', searchTerm);
    // Placeholder for search results
    setMovies([
      { id: 3, title: 'Searched Movie 1', trailerUrl: '#' },
      { id: 4, title: 'Searched Movie 2', trailerUrl: '#' },
    ]);
  };

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
              TrailerDownloader
            </Typography>
          </Toolbar>
        </AppBar>
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
          <SearchBar onSearch={handleSearch} />
          <MovieList movies={movies} />
        </Container>
        <Box component="footer" sx={{ bgcolor: 'background.paper', py: 6 }}>
          <Container maxWidth="lg">
            <Typography variant="body2" color="text.secondary" align="center">
              © {new Date().getFullYear()} TrailerDownloader
            </Typography>
          </Container>
        </Box>
      </Box>
    </ThemeProvider>
  );
}

export default App;
