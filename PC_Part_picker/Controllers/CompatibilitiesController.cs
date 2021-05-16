﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PC_Part_picker.Data;
using PC_Part_picker.Models;
using PC_Part_picker.ViewModels;

namespace PC_Part_picker.Controllers
{
    public class CompatibilitiesController : Controller
    {
        private readonly PartPickerContext _context;

        public CompatibilitiesController(PartPickerContext context)
        {
            _context = context;
        }

        // GET: Compatibilities
        public  IActionResult Index()
        {
            var compatibilites = _context.Compatibilities
                .Include(c => c.Parts).ThenInclude(p => p.Cpu)
                .Include(c => c.Parts).ThenInclude(p => p.Cooler)
                .Include(c => c.Parts).ThenInclude(p => p.Motherboard)
                .Include(c => c.Parts).ThenInclude(p => p.Ram)
                .Include(c => c.Parts).ThenInclude(p => p.Storage)
                .Include(c => c.Parts).ThenInclude(p => p.Gpu)
                .Include(c => c.Parts).ThenInclude(p => p.Psu)
                .Include(c => c.Parts).ThenInclude(p => p.Case)
                .Select(c => new CompatibilityViewModel(c))
                .ToList();

            return View(compatibilites);
        }

        // GET: Compatibilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compatibility = await _context.Compatibilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compatibility == null)
            {
                return NotFound();
            }

            return View(compatibility);
        }

        // GET: Compatibilities/Create
        public IActionResult Create()
        {
            var twoPartCompatibilities = new TwoPartCompatibilities();
            twoPartCompatibilities.AllParts = getAllParts();
            return View(twoPartCompatibilities);
        }



        // POST: Compatibilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstPartId", "SecondPartId")] TwoPartCompatibilities twoPartCompatibilities)
        {
            if (ModelState.IsValid)
            {
                var compatibility = new Compatibility();
                twoPartCompatibilities.Compatibility = compatibility;
                _context.Add(compatibility);
                var firstCompatibility = new PartCompatibility();
                firstCompatibility.Compatibility = compatibility;
                AddPartIdToCompatibility(firstCompatibility, twoPartCompatibilities.FirstPartId);
                _context.Add(firstCompatibility);
                var secondCompatibility = new PartCompatibility();
                secondCompatibility.Compatibility = compatibility;
                AddPartIdToCompatibility(secondCompatibility, twoPartCompatibilities.SecondPartId);
                _context.Add(secondCompatibility);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(twoPartCompatibilities);
        }

        private void AddPartIdToCompatibility(PartCompatibility partCompatibility, string indetification)
        {
            var splitIdentification = indetification.Split('`');
            int id = int.Parse(splitIdentification[0]);
            string name = splitIdentification[1];
            CPU cpu;
            if ((cpu = _context.Cpu.Find(id)) != null && cpu.Name == name)
            {
                partCompatibility.CPUId = id;
            }

            Cooler cooler;
            if ((cooler = _context.Cooler.Find(id)) != null && cooler.Name == name)
            {
                partCompatibility.CoolerId = id;
            }

            Motherboard motherboard;
            if ((motherboard = _context.Motherboard.Find(id)) != null && motherboard.Name == name)
            {
                partCompatibility.MotherboardId = id;
            }

            RAM ram;
            if ((ram = _context.Ram.Find(id)) != null && ram.Name == name)
            {
                partCompatibility.RAMId = id;
            }

            Storage storage;
            if ((storage = _context.Storage.Find(id)) != null && storage.Name == name)
            {
                partCompatibility.StorageId = id;
            }

            GPU gpu;
            if ((gpu = _context.Gpu.Find(id)) != null && gpu.Name == name)
            {
                partCompatibility.GPUId = id;
            }

            PSU psu;
            if ((psu = _context.Psu.Find(id)) != null && psu.Name == name)
            {
                partCompatibility.PSUId = id;
            }
            Case _case;
            if ((_case = _context.Case.Find(id)) != null && _case.Name == name)
            {
                partCompatibility.CaseId = id;
            }
        }

        // GET: Compatibilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compatibility = await _context.Compatibilities.FindAsync(id);
            if (compatibility == null)
            {
                return NotFound();
            }
            return View(compatibility);
        }

        // POST: Compatibilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] Compatibility compatibility)
        {
            if (id != compatibility.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(compatibility);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompatibilityExists(compatibility.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(compatibility);
        }

        // GET: Compatibilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var compatibility = await _context.Compatibilities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (compatibility == null)
            {
                return NotFound();
            }

            return View(compatibility);
        }

        // POST: Compatibilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var compatibility = await _context.Compatibilities.FindAsync(id);
            _context.Compatibilities.Remove(compatibility);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompatibilityExists(int id)
        {
            return _context.Compatibilities.Any(e => e.Id == id);
        }
        private List<Part> getAllParts()
        {
            var allParts = new List<Part>();
            allParts.AddRange(_context.Case.ToList());
            allParts.AddRange(_context.Cooler.ToList());
            allParts.AddRange(_context.Cpu.ToList());
            allParts.AddRange(_context.Gpu.ToList());
            allParts.AddRange(_context.Motherboard.ToList());
            allParts.AddRange(_context.Psu.ToList());
            allParts.AddRange(_context.Ram.ToList());
            return allParts;
        }
    }
}
