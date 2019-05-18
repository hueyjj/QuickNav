# QuickNav
Windows application navigator by typing instead of alt-tabbing.

# Reason for archiving
win32 is cancer. I'm just going to use switcheroo instead.

# Searching
I've tried levenshtein and jaccard algorithm, and they don't provide the results that I'm looking for. So for the moment, the searching method will
just be the following:
- Change the window title to lower case
- Try and find the user's search term as a substring
- Use indices to select one of the duplicate results, if any