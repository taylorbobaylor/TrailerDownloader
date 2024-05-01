import React, { useState, useEffect } from 'react';
import { AppBar, Toolbar, Typography, Container, Box, CssBaseline, ThemeProvider, createTheme, Card, CardContent, TextField, Button } from '@mui/material';
import axios from 'axios';
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
  onDownload: (movieId: number) => void;
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

function MovieList({ movies, onDownload }: MovieListProps) {
  return (
    <Box sx={{ my: 4 }}>
      {movies.map((movie: Movie) => (
        <Card key={movie.id} sx={{ mb: 2 }}>
          <CardContent>
            <Typography variant="h5" component="div">
              {movie.title}
            </Typography>
            <Button variant="contained" sx={{ mt: 2 }} onClick={() => onDownload(movie.id)}>
              Download Trailer
            </Button>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}

function App() {
  const [movies, setMovies] = useState<Movie[]>([]);

  useEffect(() => {
    // Fetch initial movie data on component mount
    // This can be replaced with an actual API call if needed
    setMovies([
      { id: 1, title: 'Movie 1', trailerUrl: '#' },
      { id: 2, title: 'Movie 2', trailerUrl: '#' },
    ]);
  }, []);

  const handleSearch = (searchTerm: string) => {
    axios.get(`/Movies/search?query=${encodeURIComponent(searchTerm)}`)
      .then(response => {
        setMovies(response.data);
      })
      .catch(error => {
        console.error('Error fetching search results:', error);
      });
  };

  const handleDownload = (movieId: number) => {
    axios.get(`/Movies/download/${movieId}`)
      .then(response => {
        const trailerUrl = response.data.trailerDownloadUrl;
        // This is where you would handle the actual download,
        // for example by setting the window location to the trailer URL
        window.location.href = trailerUrl;
      })
      .catch(error => {
        console.error('Error downloading trailer:', error);
      });
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
          <MovieList movies={movies} onDownload={handleDownload} />
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
