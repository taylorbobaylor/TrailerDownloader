import React from 'react';
import { AppBar, Toolbar, Typography, Container, Box, CssBaseline, ThemeProvider, createTheme, Card, CardContent } from '@mui/material';
import './App.css';

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

function MovieList() {
  // Placeholder for movie data fetching and state
  const movies = [
    { id: 1, title: 'Movie 1', trailerUrl: '#' },
    { id: 2, title: 'Movie 2', trailerUrl: '#' },
    // More movies would be fetched and listed here
  ];

  return (
    <Box sx={{ my: 4 }}>
      {movies.map((movie) => (
        <Card key={movie.id} sx={{ mb: 2 }}>
          <CardContent>
            <Typography variant="h5" component="div">
              {movie.title}
            </Typography>
            {/* Other movie details and actions can be added here */}
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}

function App() {
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
          <MovieList />
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
